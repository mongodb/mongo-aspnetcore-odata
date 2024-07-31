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
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using MongoDB.Bson;

namespace MongoDB.AspNetCore.OData;

public class MongoQueryFormatter : ExpressionVisitor
{
    private static IQueryable queryable;

    public MongoQueryFormatter(IQueryable queryable)
    {
        MongoQueryFormatter.queryable = queryable;
    }

    protected override Expression VisitMemberInit(MemberInitExpression node)
    {
        if (node.NewExpression.Type.Name == "SelectSome`1" &&
            node.Bindings[1] is MemberAssignment containerBinding &&
            containerBinding.Expression is MemberInitExpression memberInitExpression &&
            memberInitExpression.Type.Name == "NamedPropertyWithNext1`1" &&
            memberInitExpression.Bindings is IEnumerable bindings)
        {

            string nameBsonField;

            foreach (MemberAssignment binding in bindings)
            {
                if (binding.Member.Name == "Name")
                {
                    nameBsonField = ((ConstantExpression)binding.Expression).Value as string;
                }
                else if (binding.Member.Name == "Value")
                {
                }
                else if (binding.Member.Name.StartsWith("Next"))
                {
                }
            }
        }

        return base.VisitMemberInit(node);
    }
}
