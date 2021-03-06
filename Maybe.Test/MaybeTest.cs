﻿using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Maybe
{
    public class MaybeTest
    {
        public class Customer
        {
            private readonly Maybe<String> _emailAddress;

            public Customer(String emailAddress)
            {
                _emailAddress = emailAddress.Definitely();
            }

            public Customer()
            {
                _emailAddress = An.Unknown<string>();
            }

            public Maybe<string> EmailAddress
            {
                get { return _emailAddress; }
            }
        }

        [Test]
        public void EqualsOfKnownValues()
        {
            Assert.That(1.Definitely(), Is.EqualTo(1.Definitely()));
            Assert.That(1.Definitely(), Is.Not.EqualTo(2.Definitely()));
        }

        [Test]
        public void UnknownValuesAreNeverEqual()
        {
            Assert.IsFalse(An.Unknown<object>().Equals(An.Unknown<object>()));

            Maybe<object> u = An.Unknown<object>();
            Assert.IsFalse(u.Equals(u));
        }

        [Test]
        public void AnUnknownThingIsNeverEqualToAKnownThing()
        {
            Assert.That(An.Unknown<int>(), Is.Not.EqualTo(1.Definitely()));
            Assert.That(An.Unknown<string>(), Is.Not.EqualTo("rumsfeld".Definitely()));

            Assert.That(1.Definitely(), Is.Not.EqualTo(An.Unknown<int>()));
            Assert.That("rumsfeld".Definitely(), Is.Not.EqualTo(An.Unknown<string>()));
        }

        [Test]
        public void OtherwiseADefaultValue()
        {
            Assert.That(NoString().Otherwise(""), Is.EqualTo(""));
            Assert.That("foo".Definitely().Otherwise(""), Is.EqualTo("foo"));
        }

        [Test]
        public void ChainingOtherwise()
        {
            Assert.That(NoString().Otherwise(NoString()).Otherwise(""), Is.EqualTo(""));
            Assert.That(NoString().Otherwise("X".Definitely()).Otherwise(""), Is.EqualTo("X"));
            Assert.That("X".Definitely().Otherwise("Y".Definitely()).Otherwise(""), Is.EqualTo("X"));
        }

        [Test]
        public void Transforming()
        {
            Assert.That(
                new Customer("alice@example.com").EmailAddress.Select(it => it.ToUpper()).Otherwise("nobody@example.com"),
                Is.EqualTo("ALICE@EXAMPLE.COM"));
            Assert.That(new Customer().EmailAddress.Select(it => it.ToUpper()).Otherwise("UNKNOWN"),
                        Is.EqualTo("UNKNOWN"));
        }

        [Test]
        public void Querying()
        {
            Predicate<string> isValidEmailAddress = input => input.Contains("@");
            Assert.That("example@example.com".Definitely().Where(isValidEmailAddress), Is.EqualTo(true.Definitely()));
            Assert.That("invalid-email-address".Definitely().Where(isValidEmailAddress), Is.EqualTo(false.Definitely()));

            Assert.That(An.Unknown<string>().Where(isValidEmailAddress).IsKnown(), Is.EqualTo(false));
        }

        [Test]
        public void IfThen()
        {
            Maybe<string> foo = "foo".Definitely();

            if (foo.IsKnown())
                foreach (string s in foo)
                {
                    Assert.That(s, Is.EqualTo("foo"));
                }
            else
            {
                Assert.Fail("should not have been called");
            }
        }

        [Test]
        public void IfElse()
        {
            Maybe<string> foo = An.Unknown<string>();

            if (foo.IsKnown())
                foreach (String s in foo)
                {
                    Assert.Fail("should not have been called");
                }
        }

        [Test]
        public void ExampleCollectingValidEmailAddresses()
        {
            var customers = new List<Customer>
                                {
                                    new Customer(),
                                    new Customer("alice@example.com"),
                                    new Customer("bob@example.com"),
                                    new Customer(),
                                    new Customer("alice@example.com")
                                };

            var emailAddresses = new HashSet<string>(customers.SelectMany(c => c.EmailAddress));

            Assert.That(emailAddresses,
                        Is.EqualTo(new HashSet<string> {"alice@example.com", "bob@example.com", "alice@example.com"}));
        }

        [Test]
        public void IfKnownOfKnownValue()
        {
            int value = 0;
            1.Definitely().IfKnown(it => value = it);
            Assert.That(value, Is.EqualTo(1));
        }

        [Test]
        public void IfKnownOfUnknownValue()
        {
            NoString().IfKnown(it => Assert.Fail("Action was executed for an unknown object"));
        }

        private static Maybe<String> NoString()
        {
            return An.Unknown<string>();
        }
    }
}