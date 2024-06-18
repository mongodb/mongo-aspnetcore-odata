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

public class UnsafeMethodUpdater : ExpressionVisitor
{
    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        switch (node.Method.Name)
        {
            case "SubstringStart":
                {
                    MethodInfo substringMethod = typeof(string).GetMethod("Substring", new[] { typeof(int) });

                    MethodCallExpression newExpression =
                        Expression.Call(Visit(node.Arguments[0]), substringMethod, node.Arguments[1]);
                    return newExpression;
                }
            case "SubstringStartAndLength":
                {
                    MethodInfo substringWithLengthMethod =
                        typeof(string).GetMethod("Substring", new[] { typeof(int), typeof(int) });

                    MethodCallExpression newExpression = Expression.Call(Visit(node.Arguments[0]), substringWithLengthMethod,
                        node.Arguments[1], node.Arguments[2]);
                    return newExpression;
                }
            default:
                return base.VisitMethodCall(node);
        }
    }
}
