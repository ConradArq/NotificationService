﻿using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace NotificationService.API.ModelBinders.Providers
{
    public class JsonBinderProvider : IModelBinderProvider
    {
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            if (context.Metadata.BindingSource == BindingSource.Body && context.Metadata.IsComplexType && !context.Metadata.IsCollectionType)
            {
                return new JsonBinder();
            }
            return null;
        }
    }
}
