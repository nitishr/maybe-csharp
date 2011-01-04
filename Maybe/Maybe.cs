using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Maybe
{
    public static class Maybe
    {
        public static Maybe<T> Unknown<T>()
        {
            return new UnknownValue<T>();
        }

        public static Maybe<T> Definitely<T>(this T theValue)
        {
            return new DefiniteValue<T>(theValue);
        }

        private class DefiniteValue<T> : Maybe<T>
        {
            private readonly T _theValue;

            public DefiniteValue(T theValue)
            {
                _theValue = theValue;
            }

            public override bool IsKnown()
            {
                return true;
            }

            public override IEnumerator<T> GetEnumerator()
            {
                yield return _theValue;
            }

            public override T Otherwise(T defaultValue)
            {
                return _theValue;
            }

            public override Maybe<T> Otherwise(Maybe<T> maybeDefaultValue)
            {
                return this;
            }

            public override Maybe<U> Select<U>(Func<T, U> mapping)
            {
                return mapping(_theValue).Definitely();
            }

            public override Maybe<bool> Where(Predicate<T> mapping)
            {
                return mapping(_theValue).Definitely();
            }

            public override string ToString()
            {
                return "definitely " + _theValue;
            }

            public override bool Equals(object o)
            {
                return this == o ||
                       (o != null && GetType() == o.GetType() && _theValue.Equals(((DefiniteValue<T>) o)._theValue));
            }

            public override int GetHashCode()
            {
                return _theValue.GetHashCode();
            }
        }

        private class UnknownValue<T> : Maybe<T>
        {
            public override bool IsKnown()
            {
                return false;
            }

            public override IEnumerator<T> GetEnumerator()
            {
                yield break;
            }

            public override T Otherwise(T defaultValue)
            {
                return defaultValue;
            }

            public override Maybe<T> Otherwise(Maybe<T> maybeDefaultValue)
            {
                return maybeDefaultValue;
            }

            public override Maybe<U> Select<U>(Func<T, U> mapping)
            {
                return Unknown<U>();
            }

            public override Maybe<bool> Where(Predicate<T> mapping)
            {
                return Unknown<bool>();
            }

            public override string ToString()
            {
                return "unknown";
            }

            public override bool Equals(object obj)
            {
                return false;
            }

            public override int GetHashCode()
            {
                return 0;
            }
        }
    }

    public abstract class Maybe<T> : IEnumerable<T>
    {
        public abstract bool IsKnown();
        public abstract T Otherwise(T defaultValue);
        public abstract Maybe<T> Otherwise(Maybe<T> maybeDefaultValue);
        public abstract Maybe<U> Select<U>(Func<T, U> mapping);
        public abstract Maybe<bool> Where(Predicate<T> mapping);
        public abstract IEnumerator<T> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void IfKnown(Action<T> action)
        {
            this.ToList().ForEach(action);
        }
    }
}