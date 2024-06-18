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

using System.Linq.Expressions;
using System.Reflection;

namespace MongoDB.AspNetCore.OData;

public class MongoExpressionRewriter : ExpressionVisitor
{
    private readonly MethodInfo _substringWithStart = typeof(string).GetMethod("Substring", new[] { typeof(int) });
    private readonly MethodInfo _substringWithLength =
        typeof(string).GetMethod("Substring", new[] { typeof(int), typeof(int) });

    protected override Expression VisitMethodCall(MethodCallExpression node) =>
        node.Method.Name switch
        {
            // Replace oData substring methods with System substring methods to avoid incompatibility with Mongo LINQ Provider
            "SubstringStart" => Expression.Call(Visit(node.Arguments[0]), _substringWithStart,
                Visit(node.Arguments[1])),
            "SubstringStartAndLength" => Expression.Call(Visit(node.Arguments[0]), _substringWithLength,
                Visit(node.Arguments[1]), Visit(node.Arguments[2])),
            // Keep all other methods the same
            _ => base.VisitMethodCall(node)
        };
}
