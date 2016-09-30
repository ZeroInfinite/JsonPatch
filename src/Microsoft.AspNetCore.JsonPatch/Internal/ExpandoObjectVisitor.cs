﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Dynamic;
using Microsoft.AspNetCore.JsonPatch.Exceptions;

namespace Microsoft.AspNetCore.JsonPatch.Internal
{
    public class ExpandoObjectVisitor : IVisitor
    {
        public IAdapter GetAdapter(OperationContext context)
        {
            var expandoObject = context.TargetObject as ExpandoObject;
            if (expandoObject == null)
            {
                return null;
            }

            var _dictionary = (IDictionary<string, object>)context.TargetObject;

            // Example: /USStatesProperty/WA
            if (context.CurrentSegment.IsFinal)
            {
                return new ExpandoObjectAdapter((ExpandoObject)context.TargetObject, context.CurrentSegment, context.Operation);
            }
            else if (_dictionary.ContainsCaseInsensitiveKey(context.CurrentSegment))
            {
                // Example path: "/Customers/101/Address/Zipcode" and
                // let's say the current path segment is "101"
                var newTargetObject = _dictionary.GetValueForCaseInsensitiveKey(context.CurrentSegment);
                context.SetNewTargetObject(newTargetObject);
                return ObjectVisitor.GetAdapter(context);
            }

            throw new JsonPatchException(new JsonPatchError(
                context.TargetObject,
                context.Operation,
                Resources.FormatCannotPerformOperation(context.Operation.op, context.Operation.path)));
        }
    }
}