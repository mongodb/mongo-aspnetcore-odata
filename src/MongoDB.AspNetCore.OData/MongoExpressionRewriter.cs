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

    public Expression VisitSelect(MethodCallExpression node)
    {
        var source = node.Arguments[0];
        var lambda = (LambdaExpression)RemoveQuotes(node.Arguments[1]);

        // create a new lambda body using the same arguments, omitting SelectSome so that the MongoDB driver can translate it
        var newLambdaBody = VisitSelectSome(lambda);
        var newLambda = Expression.Lambda(newLambdaBody, lambda.Parameters);

        var selectMethod = typeof(Queryable).GetMethods().First(m => m.Name == "Select");
        var sourceType = source.Type.GetGenericArguments()[0];
        var newLambdaType = newLambda.ReturnType;

        var result = Expression.Call(selectMethod.MakeGenericMethod(sourceType, newLambdaType), source, newLambda);

        return result;
    }

    private static Expression RemoveQuotes(Expression e)
    {
        while (e.NodeType == ExpressionType.Quote)
        {
            e = ((UnaryExpression)e).Operand;
        }

        return e;
    }

    private Expression VisitSelectSome(LambdaExpression lambda)
    {
        var body = lambda.Body;
        var parameter = lambda.Parameters[0];

        if (body is MemberInitExpression memberInit && memberInit.NewExpression.Type.Name.StartsWith("SelectSome"))
        {
            var containerBinding = memberInit.Bindings
                .OfType<MemberAssignment>()
                .FirstOrDefault(b => b.Member.Name == "Container");

            if (containerBinding != null)
            {
                var containerInit = (MemberInitExpression)containerBinding.Expression;
                var containerBindings = containerInit.Bindings.OfType<MemberAssignment>().ToList();

                var idBinding = containerBindings.First(b => b.Member.Name == "Value"
                                                             && b.Expression is ConditionalExpression);
                var nameBinding = containerBindings.First(b => b.Member.Name == "Next0"
                                                               && b.Expression is MemberInitExpression);
                var areaBinding = containerBindings.First(b => b.Member.Name == "Next1"
                                                               && b.Expression is MemberInitExpression);

                var idProperty = ((UnaryExpression)((ConditionalExpression)idBinding.Expression).IfFalse).Operand;
                // MemberInfo idInfo = ((MemberExpression)idProperty).Member;
                MemberInfo idInfo = typeof(AnonymousType).GetProperty(nameof(AnonymousType.Id));
                var newIdBinding = Expression.Bind(idInfo, idProperty);

                var nameProperty =
                    ((ConditionalExpression)
                        ((MemberAssignment)((MemberInitExpression)nameBinding.Expression).Bindings[1]).Expression)
                    .IfFalse;
                // MemberInfo nameInfo = ((MemberExpression)nameProperty).Member;
                MemberInfo nameInfo = typeof(AnonymousType).GetProperty(nameof(AnonymousType.Name));
                var newNameBinding = Expression.Bind(nameInfo, nameProperty);

                var areaProperty = ((MemberAssignment)((MemberInitExpression)areaBinding.Expression).Bindings[1])
                    .Expression;
                // MemberInfo areaInfo = ((MemberAssignment)((MemberInitExpression)areaBinding.Expression).Bindings[1])
                //     .Member;
                MemberInfo areaInfo = typeof(AnonymousType).GetProperty(nameof(AnonymousType.Area));
                var newAreaBinding = Expression.Bind(areaInfo, areaProperty);

                var newBindings = new[] { newIdBinding, newNameBinding, newAreaBinding };

                return Expression.MemberInit(Expression.New(typeof(AnonymousType)), newBindings);
            }
        }

        return body;
    }

    private class AnonymousType
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public double Area { get; set; }
    }
}
