using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace HondataDotNet.Datalog.OBDII
{
    public abstract class ReadinessCodeDictionary<TReadinessTest, TReadinessCode> : IReadOnlyDictionary<TReadinessTest, TReadinessCode>
        where TReadinessTest : struct, Enum
        where TReadinessCode : IReadinessCode<TReadinessTest>
    {
        public abstract TReadinessCode this[TReadinessTest key] { get; }

        public virtual IEnumerable<TReadinessTest> Keys => Enum.GetValues(typeof(TReadinessTest)).Cast<TReadinessTest>();

        public virtual IEnumerable<TReadinessCode> Values
        {
            get
            {
                foreach (var key in Keys)
                {
                    yield return this[key];
                }
            }
        }
        public virtual int Count => Keys.Count();

        public virtual bool ContainsKey(TReadinessTest key)
        {
            return Keys.Contains(key);
        }

        public virtual IEnumerator<KeyValuePair<TReadinessTest, TReadinessCode>> GetEnumerator()
        {
            foreach (var key in Keys)
            {
                yield return new(key, this[key]);
            }
        }

#pragma warning disable CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).
        public virtual bool TryGetValue(TReadinessTest key, [MaybeNullWhen(false)] out TReadinessCode value)
#pragma warning restore CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).
        {
            if (!ContainsKey(key))
            {
                value = default;
                return false;
            }
            value = this[key];
            return true;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}