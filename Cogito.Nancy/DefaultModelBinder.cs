﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Nancy.ModelBinding;

namespace Cogito.Nancy
{

    /// <summary>
    /// <see cref="DefaultBinder"/> exposed as a <see cref="IModelBinder"/> so that it operates regardless of fallback.
    /// </summary>
    [Export(typeof(IModelBinder))]
    public class DefaultModelBinder : DefaultBinder, IModelBinder
    {

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="typeConverters"></param>
        /// <param name="bodyDeserializers"></param>
        /// <param name="fieldNameConverter"></param>
        /// <param name="defaults"></param>
        [ImportingConstructor]
        public DefaultModelBinder(
            [ImportMany] IEnumerable<ITypeConverter> typeConverters,
            [ImportMany] IEnumerable<IBodyDeserializer> bodyDeserializers,
            IFieldNameConverter fieldNameConverter,
            BindingDefaults defaults)
            : base(typeConverters, bodyDeserializers, fieldNameConverter, defaults)
        {

        }

        public bool CanBind(Type modelType)
        {
            return true;
        }

    }

}
