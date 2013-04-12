﻿using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.WindowsAzure.Storage.Table;
using WindowsAzure.Properties;
using WindowsAzure.Table.Attributes;
using WindowsAzure.Table.EntityConverters.TypeData.ValueAccessors;

namespace WindowsAzure.Table.EntityConverters.TypeData.Properties
{
    /// <summary>
    ///     Handles access to PartitionKey value.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    public sealed class PartitionKeyAccessor<T> : IKeyProperty<T>
    {
        private readonly Type _partitionKeyAttributeType;
        private readonly Type _stringType;
        private IValueAccessor<T> _accessor;

        /// <summary>
        ///     Constructor.
        /// </summary>
        public PartitionKeyAccessor()
        {
            _partitionKeyAttributeType = typeof (PartitionKeyAttribute);
            _stringType = typeof (String);

            NameChanges = new Dictionary<string, string>();
        }

        public bool HasAccessor
        {
            get { return _accessor != null; }
        }

        public bool Validate(MemberInfo memberInfo, IValueAccessor<T> accessor)
        {
            // Checks member for a partition key attribute
            if (!Attribute.IsDefined(memberInfo, _partitionKeyAttributeType, false))
            {
                return false;
            }

            if (_accessor != null)
            {
                throw new ArgumentException(Resources.PartitionKeyAttributeDuplicate);
            }

            if (accessor.Type != _stringType)
            {
                throw new ArgumentException(Resources.PartitionKeyInvalidType);
            }

            _accessor = accessor;

            NameChanges.Add(accessor.Name, "PartitionKey");

            return true;
        }

        public void FillEntity(DynamicTableEntity tableEntity, T entity)
        {
            if (_accessor != null)
            {
                _accessor.SetValue(entity, new EntityProperty(tableEntity.PartitionKey));
            }
        }

        public void FillTableEntity(T entity, DynamicTableEntity tableEntity)
        {
            if (_accessor != null)
            {
                tableEntity.PartitionKey = _accessor.GetValue(entity).StringValue;
            }

            tableEntity.PartitionKey = tableEntity.PartitionKey ?? string.Empty;
        }

        public IDictionary<string, string> NameChanges { get; private set; }
    }
}