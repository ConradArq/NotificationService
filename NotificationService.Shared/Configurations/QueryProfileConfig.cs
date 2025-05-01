﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace NotificationService.Shared.Configurations
{
    /// <summary>
    /// A static registry for configuring and retrieving custom property-level query mappings for entities.
    /// Used to dynamically build LINQ expressions when filtering by properties that require special comparison logic.
    /// Not needed for basic queries where filter property names match entity property names exactly.
    /// </summary>
    public static class QueryProfileConfig
    {
        // Dictionary to hold mappings for each entity type
        private static readonly Dictionary<Type, EntityMapping> Mappings = new();

        // Register configuration for a specific entity type using fluent-style chaining
        public static void Register<T>(Action<EntityMapping> mappingConfig)
        {
            var mapping = new EntityMapping();
            mappingConfig(mapping);
            Mappings[typeof(T)] = mapping;
        }

        // Retrieve the mapping for a specific entity type
        public static EntityMapping? GetMapping<T>()
        {
            Mappings.TryGetValue(typeof(T), out var mapping);
            return mapping;
        }

        // Method to allow loading mappings from external configuration profiles
        public static void ApplyMappings(Action configure)
        {
            configure();
        }
    }

    /// <summary>
    /// Holds custom comparison logic for individual properties of a given entity type.
    /// Each property can be mapped to a custom expression builder (e.g., Contains, Equals, etc.).
    /// </summary>
    public class EntityMapping
    {
        private readonly Dictionary<string, Func<Expression, object, Expression>> _customComparisons = new();

        // Add a custom comparison function for a specific property
        public EntityMapping ForProperty(string propertyName, Func<Expression, object, Expression> comparisonFunc)
        {
            _customComparisons[propertyName] = comparisonFunc;
            return this;
        }

        // Try to retrieve a custom comparison function for a property
        public bool TryGetCustomComparison(string propertyName, out Func<Expression, object, Expression> comparisonFunc)
        {
            return _customComparisons.TryGetValue(propertyName, out comparisonFunc!);
        }
    }
}
