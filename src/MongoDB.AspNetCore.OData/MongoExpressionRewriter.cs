// Copyright 2023-present MongoDB Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License")
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using MongoDB.Bson;

namespace MongoDB.AspNetCore.OData;

internal class MongoExpressionRewriter : ExpressionVisitor
{
    private readonly MethodInfo _substringWithStart = typeof(string).GetMethod("Substring", new[] { typeof(int) });
    private readonly MethodInfo _substringWithLength =
        typeof(string).GetMethod("Substring", new[] { typeof(int), typeof(int) });

    protected override Expression VisitMethodCall(MethodCallExpression node) =>
        node.Method.Name switch
        {
            "SubstringStart" => Expression.Call(Visit(node.Arguments[0]), _substringWithStart,
                Visit(node.Arguments[1])),
            "SubstringStartAndLength" => Expression.Call(Visit(node.Arguments[0]), _substringWithLength,
                Visit(node.Arguments[1]), Visit(node.Arguments[2])),
            "Select" => VisitSelect(node),
            _ => base.VisitMethodCall(node)
        };

    private static Expression VisitSelect(MethodCallExpression node)
    {
        var source = node.Arguments[0];
        var lambda = (LambdaExpression)RemoveQuotes(node.Arguments[1]);

        // create a new lambda body using the same arguments, omitting SelectSome so that the MongoDB driver can translate it
        var newLambdaBody = VisitSelectBson(lambda);
        var newLambda = Expression.Lambda(newLambdaBody, lambda.Parameters);

        var selectMethod = typeof(Queryable).GetMethods().First(m => m.Name == "Select");
        var sourceType = source.Type.GetGenericArguments()[0];
        var newLambdaType = newLambda.ReturnType;

        var result = Expression.Call(selectMethod.MakeGenericMethod(sourceType, newLambdaType), source, newLambda);

        return result;
    }

    public static Expression RemoveQuotes(Expression e)
    {
        while (e.NodeType == ExpressionType.Quote)
        {
            e = ((UnaryExpression)e).Operand;
        }

        return e;
    }

    private static Expression VisitSelectBson(LambdaExpression lambda)
    {
        var body = lambda.Body;

        if (body is MemberInitExpression memberInit && memberInit.NewExpression.Type.Name.StartsWith("SelectSome"))
        {
            var containerBinding = memberInit.Bindings[1] as MemberAssignment;

            var containerInit = (MemberInitExpression)containerBinding.Expression;
            var containerBindings = containerInit.Bindings.OfType<MemberAssignment>().ToList();

            ElementInit[] elements = new ElementInit[containerBindings.Count - 1];
            MethodInfo addMethod =
                typeof(BsonDocument).GetMethod("Add",
                    new[] { typeof(string), typeof(BsonValue) }); // TODO: change to 2 args

            for (int i = 0; i < containerBindings.Count; i++)
            {
                MemberAssignment currentBinding = containerBindings[i];

                if (currentBinding.Member.Name == "Name")
                {
                    var valueBinding = containerBindings[i + 1];

                    var propertyName = currentBinding.Expression as ConstantExpression;
                    var propertyValue = Expression.Convert(valueBinding.Expression, typeof(BsonValue));

                    var elementInit =
                        Expression.ElementInit(addMethod, new Expression[] { propertyName, propertyValue });
                    elements[i] = elementInit;
                }
                else if (currentBinding.Member.Name == "Value")
                {
                    // Skip, already handled above
                }
                else if (currentBinding.Member.Name.StartsWith("Next"))
                {
                    if (currentBinding.Expression is MemberInitExpression nextInit &&
                        nextInit.Bindings[0] is MemberAssignment keyBinding &&
                        nextInit.Bindings[1] is MemberAssignment valueBinding)
                    {
                        var propertyName = keyBinding.Expression as ConstantExpression;
                        var propertyValue = Expression.Convert(valueBinding.Expression, typeof(BsonValue));

                        var elementInit =
                            Expression.ElementInit(addMethod, new Expression[] { propertyName, propertyValue });
                        elements[i - 1] = elementInit;
                    }
                }
                else
                {
                    throw new NotSupportedException($"Unsupported binding type: {currentBinding.Member.Name}");
                }
            }

            NewExpression newExpr = Expression.New(typeof(BsonDocument));
            ListInitExpression listInitExpression = Expression.ListInit(newExpr, elements);
            return listInitExpression;
        }

        return body;
    }
}
