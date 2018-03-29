using System;
using System.Collections.Generic;
using Dependencies;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace DependencyGraphTestCases
{
    /// <summary>
    /// IMPORTANT NOTICE: My code coverage appears low because "Assert.Fail();" is never covered
    /// in these tests since they all pass. However, you'll notice that the code coverage in the 
    /// DependencyGraph class alone is near 100%.
    /// </summary>

    [TestClass]
    public class UnitTest1
    {
        //ALL OF MY PERSONAL TESTS ARE FIRST. THE PROFESSOR'S TESTS APPEAR AFER MINE

        /// <summary>
        /// This was my first test, creating one dependency and making sure the size is right
        /// </summary>
        [TestMethod]
        public void AddOnce()
        {
            DependencyGraph testing = new DependencyGraph();
            testing.AddDependency("a", "b");
            if (!(testing.Size == 1))
                Assert.Fail();
        }

        /// <summary>
        /// Makes sure that nothing crazy happens when we remove a dependency that doesn't exist.
        /// </summary>
        [TestMethod]
        public void RemoveWithNothing()
        {
            DependencyGraph testing = new DependencyGraph();
            testing.RemoveDependency("a", "b");
            if (!(testing.Size == 0))
                Assert.Fail();
        }

        /// <summary>
        /// Makes sure that AddDependency throws an exception with null parameters
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddEmptyDependency()
        {
            DependencyGraph testing = new DependencyGraph();
            testing.AddDependency(null, null);
        }

        /// <summary>
        /// This makes sure that something that doesn't have dependents returns false
        /// </summary>
        [TestMethod]
        public void HasDependentsFALSE()
        {
            DependencyGraph testing = new DependencyGraph();
            testing.AddDependency("a", "b");
            if (testing.HasDependents("b"))
                Assert.Fail();
        }
        /// <summary>
        /// This makes sure that something that has dependents returns true
        /// </summary>
        [TestMethod]
        public void HasDependentsTRUE()
        {
            DependencyGraph testing = new DependencyGraph();
            testing.AddDependency("a", "b");
            if (!(testing.HasDependents("a")))
                Assert.Fail();
        }


        /// <summary>
        /// This makes sure that null throws and ArgumentNullException
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void HasDependentsNull()
        {
            DependencyGraph testing = new DependencyGraph();
            testing.AddDependency("a", "b");
            testing.HasDependents(null);
        }

        /// <summary>
        /// Makes sure something without dependeees returns false
        /// </summary>
        [TestMethod]
        public void HasDependees()
        {
            DependencyGraph testing = new DependencyGraph();
            testing.AddDependency("a", "b");
            if (testing.HasDependees("a"))
                Assert.Fail();
        }

        /// <summary>
        /// Makes sure that something with dependees returns true
        /// </summary>
        [TestMethod]
        public void HasDependeesTRUE()
        {
            DependencyGraph testing = new DependencyGraph();
            testing.AddDependency("a", "b");
            if (!(testing.HasDependees("b")))
                Assert.Fail();
        }

        /// <summary>
        /// Makes sure that a null throws an argument null exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void HasDependeesNull()
        {
            DependencyGraph testing = new DependencyGraph();
            testing.AddDependency("a", "b");
            testing.HasDependees(null);
        }

        /// <summary>
        /// Makes sure that a null throws an argument null exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullParameterConstructingGraph()
        {
            DependencyGraph testing = new DependencyGraph(null);
            Assert.Fail();
        }

        /// <summary>
        /// Makes sure that a graph as a parameter works okay.
        /// </summary>
        [TestMethod]
        public void ParameterConstructingGraph()
        {
            DependencyGraph original = new DependencyGraph();
            original.AddDependency("a", "b");
            original.AddDependency("c", "e");
            DependencyGraph testing = new DependencyGraph(original);
            if (testing.Size != original.Size)
                Assert.Fail();
        }

        /// <summary>
        /// Makes sure that a single remove function works with size
        /// </summary>
        [TestMethod]
        public void AddRemoveOnce()
        {
            DependencyGraph testing = new DependencyGraph();
            testing.AddDependency("a", "b");
            testing.RemoveDependency("a", "b");
            if (!(testing.Size == 0))
                Assert.Fail();
        }

        /// <summary>
        /// Makes sure that a duplicate entry doesn't do anything
        /// </summary>
        [TestMethod]
        public void Duplicate()
        {
            DependencyGraph testing = new DependencyGraph();
            testing.AddDependency("a", "b");
            testing.AddDependency("a", "b");
            if (!(testing.Size == 1))
                Assert.Fail();
        }

        /// <summary>
        /// Makes sure that non duplicate entries work
        /// </summary>
        [TestMethod]
        public void NonDuplicate()
        {
            DependencyGraph testing = new DependencyGraph();
            testing.AddDependency("a", "b");
            testing.AddDependency("a", "c");
            if (!(testing.Size == 2))
                Assert.Fail();
        }

        /// <summary>
        /// Makes sure that the has dependency method works after one entry
        /// </summary>
        [TestMethod]
        public void SimpleHasDepents()
        {
            DependencyGraph testing = new DependencyGraph();
            testing.AddDependency("a", "b");
            if (!(testing.HasDependents("a")))
                Assert.Fail();
        }

        /// <summary>
        /// Makes sure that a remove works with hasDependents
        /// </summary>
        [TestMethod]
        public void SimpleHasDepents2()
        {
            DependencyGraph testing = new DependencyGraph();
            testing.AddDependency("a", "b");
            testing.RemoveDependency("a", "b");
            if (testing.HasDependents("a"))
                Assert.Fail();
        }

        /// <summary>
        /// Check that null throws exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetDependentsNONE()
        {
            DependencyGraph testing = new DependencyGraph();
            testing.AddDependency("a", "b");
            testing.RemoveDependency("a", "b");
            testing.GetDependents(null).Count<string>();
        }

        /// <summary>
        /// Checks that null throws the right exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetDependeesNONE()
        {
            DependencyGraph testing = new DependencyGraph();
            testing.AddDependency("a", "b");
            testing.RemoveDependency("a", "b");
            testing.GetDependees(null).Count<string>();
        }

        /// <summary>
        /// Makes sure that removing a dependency doesn't work with null and throws exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RemoveDependencyWithTNull()
        {
            DependencyGraph testing = new DependencyGraph();
            testing.AddDependency("a", "b");
            testing.RemoveDependency("a", null);
        }

        /// <summary>
        /// Makes sure that removeDependency works
        /// </summary>
        [TestMethod]
        public void CorrectDependents()
        {
            DependencyGraph testing = new DependencyGraph();
            testing.AddDependency("a", "b");
            testing.AddDependency("a", "c");
            testing.AddDependency("a", "c");
            testing.AddDependency("a", "d");
            testing.RemoveDependency("a", "c");
            var x = new string[] { "b", "d" };
            var y = testing.GetDependents("a").ToArray<string>();

            if (testing.Size != 2)
                Assert.Fail();

            for (int a = 0; a < y.Length; a++)
            {
                if (!(x[a].Equals(y[a])))
                    Assert.Fail();
            }
        }

        /// <summary>
        /// Make sure that removeDependees works
        /// </summary>
        [TestMethod]
        public void CorrectDependees()
        {
            DependencyGraph testing = new DependencyGraph();
            testing.AddDependency("b", "c");
            testing.AddDependency("a", "c");
            testing.AddDependency("a", "c");
            testing.AddDependency("e", "c");
            testing.RemoveDependency("a", "c");
            var x = new string[] { "b", "e" };
            var y = testing.GetDependees("c").ToArray<string>();

            if (testing.Size != 2)
                Assert.Fail();

            for (int a = 0; a < y.Length; a++)
            {
                if (!(x[a].Equals(y[a])))
                    Assert.Fail();
            }
        }

        /// <summary>
        /// Makes sure that dependees works in a different way
        /// </summary>
        [TestMethod]
        public void CorrectDependees2()
        {
            DependencyGraph testing = new DependencyGraph();
            testing.AddDependency("b", "c");
            testing.AddDependency("a", "e");
            testing.AddDependency("a", "c");
            testing.AddDependency("e", "c");
            testing.RemoveDependency("a", "c");
            var x = new string[] { "b", "e" };
            var y = testing.GetDependees("c").ToArray<string>();

            if (testing.Size != 3)
                Assert.Fail();

            for (int a = 0; a < y.Length; a++)
            {
                if (!(x[a].Equals(y[a])))
                    Assert.Fail();
            }
        }

        /// <summary>
        /// Creates 200,000 dependencies and removes almost all of them in well under a second
        /// </summary>
        [TestMethod]
        public void STRESSTEST_200k()
        {
            DependencyGraph testing = new DependencyGraph();
            for (int a = 0; a < 200000; a++)
                testing.AddDependency("a", a.ToString());

            for (int b = 0; b < 199998; b++)
                testing.RemoveDependency("a", b.ToString());

            if (!(testing.Size == 2))
                Assert.Fail();
        }

        /// <summary>
        /// Makes sure that replace dependence works with size and the actual values
        /// </summary>
        [TestMethod]
        public void ReplaceDependents1()
        {
            DependencyGraph testing = new DependencyGraph();
            testing.AddDependency("b", "d");
            testing.AddDependency("a", "a");
            testing.AddDependency("a", "b");
            testing.AddDependency("a", "c");
            testing.AddDependency("a", "c");
            testing.AddDependency("a", "d");

            string[] replacements = new string[] { "f", "g", "h", "i", "j", "k" };
            testing.ReplaceDependents("a", replacements);

            var y = testing.GetDependents("a").ToArray<string>();

            if (testing.Size != 7)
                Assert.Fail();

            for (int a = 0; a < y.Length; a++)
            {
                if (!(replacements[a].Equals(y[a])))
                    Assert.Fail();
            }
        }

        /// <summary>
        /// Makes sure that replace dependees works with size and actual values
        /// </summary>
        [TestMethod]
        public void ReplaceDependees1()
        {
            DependencyGraph testing = new DependencyGraph();
            testing.AddDependency("a", "b");
            testing.AddDependency("a", "a");
            testing.AddDependency("b", "a");
            testing.AddDependency("c", "a");
            testing.AddDependency("d", "a");
            testing.AddDependency("e", "a");
            testing.AddDependency("f", "a");
            testing.AddDependency("g", "a");
            testing.AddDependency("h", "a");

            string[] replacements = new string[] { "f", "g", "h", "i" };
            testing.ReplaceDependees("a", replacements);

            var y = testing.GetDependees("a").ToArray<string>();

            if (testing.Size != 5)
                Assert.Fail();

            for (int a = 0; a < y.Length; a++)
            {
                if (!(replacements[a].Equals(y[a])))
                    Assert.Fail();
            }
        }

        /// <summary>
        /// Makes sure that replace dependees throws ArgumentNullException with a null parameter
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReplaceDependeesNull()
        {
            DependencyGraph testing = new DependencyGraph();
            testing.AddDependency("a", "b");
            testing.AddDependency("a", "a");
            testing.AddDependency("b", "a");
            testing.AddDependency("c", "a");
            testing.AddDependency("d", "a");
            testing.AddDependency("e", "a");
            testing.AddDependency("f", "a");
            testing.AddDependency("g", "a");
            testing.AddDependency("h", "a");

            string[] replacements = new string[] { "f", "g", "h", "i" };
            testing.ReplaceDependees(null, null);
        }

        /// <summary>
        /// Makes sure that replace dependents throws ArgumentNullException with a null parameter
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReplaceDependentsNull()
        {
            DependencyGraph testing = new DependencyGraph();
            testing.AddDependency("a", "b");
            testing.AddDependency("a", "a");
            testing.AddDependency("b", "a");
            testing.AddDependency("c", "a");
            testing.AddDependency("d", "a");
            testing.AddDependency("e", "a");
            testing.AddDependency("f", "a");
            testing.AddDependency("g", "a");
            testing.AddDependency("h", "a");

            string[] replacements = new string[] { "f", "g", "h", "i" };
            testing.ReplaceDependents(null, null);
        }

        /// <summary>
        /// This tests adding, removing, and replacing dependents and dependees. I check the size, but I also
        /// personally debugged to check the values in each dictionary to check for accuracy.
        /// </summary>
        [TestMethod]
        public void TestingAlmostEverything()
        {
            DependencyGraph testing = new DependencyGraph();
            testing.AddDependency("a", "b"); //1
            testing.AddDependency("b", "c"); //2
            testing.AddDependency("a", "c"); //3
            testing.AddDependency("z", "c"); //4
            testing.AddDependency("a", "b"); //4
            testing.RemoveDependency("a", "b"); //3
            HashSet<string> replacementDependents = new HashSet<string> { "g", "h", "i", "j", "k" }; //7
            testing.ReplaceDependents("a", replacementDependents);
            HashSet<string> replacementDependees = new HashSet<string> { "r", "s", "t", "u", "v" }; //11
            testing.ReplaceDependees("k", replacementDependees);

            if (testing.Size != 11)
                Assert.Fail();


        }

        //THE TESTS ABOVE ARE ALL MY OWN TESTS----------------------------------------------------------------------------------------------
        //THE TESTS BELOW ARE ALL THE PROFESSOR'S' TESTS UNTOUCHED--------------------------------------------------------------------------


        // ************************** TESTS ON EMPTY DGs ************************* //

        /// <summary>
        ///Empty graph should contain nothing
        ///</summary>
        [TestMethod()]
        public void EmptyTest1()
        {
            DependencyGraph t = new DependencyGraph();
            Assert.AreEqual(0, t.Size);
        }

        /// <summary>
        ///Empty graph should contain nothing
        ///</summary>
        [TestMethod()]
        public void EmptyTest2()
        {
            DependencyGraph t = new DependencyGraph();
            Assert.IsFalse(t.HasDependees("x"));
            Assert.IsFalse(t.HasDependents("x"));
        }

        /// <summary>
        ///Empty graph should contain nothing
        ///</summary>
        [TestMethod()]
        public void EmptyTest3()
        {
            DependencyGraph t = new DependencyGraph();
            Assert.IsFalse(t.GetDependees("x").GetEnumerator().MoveNext());
            Assert.IsFalse(t.GetDependents("x").GetEnumerator().MoveNext());
        }

        /// <summary>
        ///Removing from an empty DG shouldn't fail
        ///</summary>
        [TestMethod()]
        public void EmptyTest5()
        {
            DependencyGraph t = new DependencyGraph();
            t.RemoveDependency("x", "y");
        }

        /// <summary>
        ///Replace on an empty DG shouldn't fail
        ///</summary>
        [TestMethod()]
        public void EmptyTest6()
        {
            DependencyGraph t = new DependencyGraph();
            t.ReplaceDependents("x", new HashSet<string>());
            t.ReplaceDependees("y", new HashSet<string>());
        }


        // ************************ MORE TESTS ON EMPTY DGs *********************** //

        /// <summary>
        ///Empty graph should contain nothing
        ///</summary>
        [TestMethod()]
        public void EmptyTest7()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "y");
            Assert.AreEqual(1, t.Size);
            t.RemoveDependency("x", "y");
            Assert.AreEqual(0, t.Size);
        }

        /// <summary>
        ///Empty graph should contain nothing
        ///</summary>
        [TestMethod()]
        public void EmptyTest8()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "y");
            Assert.IsTrue(t.HasDependees("y"));
            Assert.IsTrue(t.HasDependents("x"));
            t.RemoveDependency("x", "y");
            Assert.IsFalse(t.HasDependees("y"));
            Assert.IsFalse(t.HasDependents("x"));
        }

        /// <summary>
        ///Empty graph should contain nothing
        ///</summary>
        [TestMethod()]
        public void EmptyTest9()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "y");
            IEnumerator<string> e1 = t.GetDependees("y").GetEnumerator();
            Assert.IsTrue(e1.MoveNext());
            Assert.AreEqual("x", e1.Current);
            IEnumerator<string> e2 = t.GetDependents("x").GetEnumerator();
            Assert.IsTrue(e2.MoveNext());
            Assert.AreEqual("y", e2.Current);
            t.RemoveDependency("x", "y");
            Assert.IsFalse(t.GetDependees("y").GetEnumerator().MoveNext());
            Assert.IsFalse(t.GetDependents("x").GetEnumerator().MoveNext());
        }

        /// <summary>
        ///Removing from an empty DG shouldn't fail
        ///</summary>
        [TestMethod()]
        public void EmptyTest11()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "y");
            Assert.AreEqual(t.Size, 1);
            t.RemoveDependency("x", "y");
            t.RemoveDependency("x", "y");
        }

        /// <summary>
        ///Replace on an empty DG shouldn't fail
        ///</summary>
        [TestMethod()]
        public void EmptyTest12()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "y");
            Assert.AreEqual(t.Size, 1);
            t.RemoveDependency("x", "y");
            t.ReplaceDependents("x", new HashSet<string>());
            t.ReplaceDependees("y", new HashSet<string>());
        }


        // ********************** Making Sure that Static Variables Weren't Used ****************** //
        ///<summary>
        ///It should be possibe to have more than one DG at a time.  This test is
        ///repeated because I want it to be worth more than 1 point.
        ///</summary>
        [TestMethod()]
        public void StaticTest1()
        {
            DependencyGraph t1 = new DependencyGraph();
            DependencyGraph t2 = new DependencyGraph();
            t1.AddDependency("x", "y");
            Assert.AreEqual(1, t1.Size);
            Assert.AreEqual(0, t2.Size);
        }

        [TestMethod()]
        public void StaticTest2()
        {
            StaticTest1();
        }

        [TestMethod()]
        public void StaticTest3()
        {
            StaticTest1();
        }

        [TestMethod()]
        public void StaticTest4()
        {
            StaticTest1();
        }

        [TestMethod()]
        public void StaticTest5()
        {
            StaticTest1();
        }

        /**************************** SIMPLE NON-EMPTY TESTS ****************************/

        /// <summary>
        ///Non-empty graph contains something
        ///</summary>
        [TestMethod()]
        public void NonEmptyTest1()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("a", "c");
            t.AddDependency("c", "b");
            t.AddDependency("b", "d");
            Assert.AreEqual(4, t.Size);
        }

        /// <summary>
        ///Non-empty graph contains something
        ///</summary>
        [TestMethod()]
        public void NonEmptyTest3()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("a", "c");
            t.AddDependency("c", "b");
            t.AddDependency("b", "d");
            Assert.IsTrue(t.HasDependents("a"));
            Assert.IsFalse(t.HasDependees("a"));
            Assert.IsTrue(t.HasDependents("b"));
            Assert.IsTrue(t.HasDependees("b"));
        }

        /// <summary>
        ///Non-empty graph contains something
        ///</summary>
        [TestMethod()]
        public void NonEmptyTest4()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("a", "c");
            t.AddDependency("c", "b");
            t.AddDependency("b", "d");

            IEnumerator<string> e = t.GetDependees("a").GetEnumerator();
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependees("b").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            String s1 = e.Current;
            Assert.IsTrue(e.MoveNext());
            String s2 = e.Current;
            Assert.IsFalse(e.MoveNext());
            Assert.IsTrue(((s1 == "a") && (s2 == "c")) || ((s1 == "c") && (s2 == "a")));

            e = t.GetDependees("c").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("a", e.Current);
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependees("d").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("b", e.Current);
            Assert.IsFalse(e.MoveNext());
        }

        /// <summary>
        ///Non-empty graph contains something
        ///</summary>
        [TestMethod()]
        public void NonEmptyTest5()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("a", "c");
            t.AddDependency("c", "b");
            t.AddDependency("b", "d");

            IEnumerator<string> e = t.GetDependents("a").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            String s1 = e.Current;
            Assert.IsTrue(e.MoveNext());
            String s2 = e.Current;
            Assert.IsFalse(e.MoveNext());
            Assert.IsTrue(((s1 == "b") && (s2 == "c")) || ((s1 == "c") && (s2 == "b")));

            e = t.GetDependents("b").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("d", e.Current);
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependents("c").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("b", e.Current);
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependents("d").GetEnumerator();
            Assert.IsFalse(e.MoveNext());
        }

        /// <summary>
        ///Non-empty graph contains something
        ///</summary>
        [TestMethod()]
        public void NonEmptyTest6()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("a", "c");
            t.AddDependency("a", "b");
            t.AddDependency("c", "b");
            t.AddDependency("b", "d");
            t.AddDependency("c", "b");
            Assert.AreEqual(4, t.Size);
        }

        /// <summary>
        ///Non-empty graph contains something
        ///</summary>
        [TestMethod()]
        public void NonEmptyTest8()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("a", "c");
            t.AddDependency("a", "b");
            t.AddDependency("c", "b");
            t.AddDependency("b", "d");
            t.AddDependency("c", "b");
            Assert.IsTrue(t.HasDependents("a"));
            Assert.IsFalse(t.HasDependees("a"));
            Assert.IsTrue(t.HasDependents("b"));
            Assert.IsTrue(t.HasDependees("b"));
        }

        /// <summary>
        ///Non-empty graph contains something
        ///</summary>
        [TestMethod()]
        public void NonEmptyTest9()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("a", "c");
            t.AddDependency("a", "b");
            t.AddDependency("c", "b");
            t.AddDependency("b", "d");
            t.AddDependency("c", "b");

            IEnumerator<string> e = t.GetDependees("a").GetEnumerator();
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependees("b").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            String s1 = e.Current;
            Assert.IsTrue(e.MoveNext());
            String s2 = e.Current;
            Assert.IsFalse(e.MoveNext());
            Assert.IsTrue(((s1 == "a") && (s2 == "c")) || ((s1 == "c") && (s2 == "a")));

            e = t.GetDependees("c").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("a", e.Current);
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependees("d").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("b", e.Current);
            Assert.IsFalse(e.MoveNext());
        }

        /// <summary>
        ///Non-empty graph contains something
        ///</summary>
        [TestMethod()]
        public void NonEmptyTest10()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("a", "c");
            t.AddDependency("a", "b");
            t.AddDependency("c", "b");
            t.AddDependency("b", "d");
            t.AddDependency("c", "b");

            IEnumerator<string> e = t.GetDependents("a").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            String s1 = e.Current;
            Assert.IsTrue(e.MoveNext());
            String s2 = e.Current;
            Assert.IsFalse(e.MoveNext());
            Assert.IsTrue(((s1 == "b") && (s2 == "c")) || ((s1 == "c") && (s2 == "b")));

            e = t.GetDependents("b").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("d", e.Current);
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependents("c").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("b", e.Current);
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependents("d").GetEnumerator();
            Assert.IsFalse(e.MoveNext());
        }

        /// <summary>
        ///Non-empty graph contains something
        ///</summary>
        [TestMethod()]
        public void NonEmptyTest11()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "y");
            t.AddDependency("a", "b");
            t.AddDependency("a", "c");
            t.AddDependency("a", "d");
            t.AddDependency("c", "b");
            t.RemoveDependency("a", "d");
            t.AddDependency("e", "b");
            t.AddDependency("b", "d");
            t.RemoveDependency("e", "b");
            t.RemoveDependency("x", "y");
            Assert.AreEqual(4, t.Size);
        }

        /// <summary>
        ///Non-empty graph contains something
        ///</summary>
        [TestMethod()]
        public void NonEmptyTest13()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "y");
            t.AddDependency("a", "b");
            t.AddDependency("a", "c");
            t.AddDependency("a", "d");
            t.AddDependency("c", "b");
            t.RemoveDependency("a", "d");
            t.AddDependency("e", "b");
            t.AddDependency("b", "d");
            t.RemoveDependency("e", "b");
            t.RemoveDependency("x", "y");
            Assert.IsTrue(t.HasDependents("a"));
            Assert.IsFalse(t.HasDependees("a"));
            Assert.IsTrue(t.HasDependents("b"));
            Assert.IsTrue(t.HasDependees("b"));
        }

        /// <summary>
        ///Non-empty graph contains something
        ///</summary>
        [TestMethod()]
        public void NonEmptyTest14()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "y");
            t.AddDependency("a", "b");
            t.AddDependency("a", "c");
            t.AddDependency("a", "d");
            t.AddDependency("c", "b");
            t.RemoveDependency("a", "d");
            t.AddDependency("e", "b");
            t.AddDependency("b", "d");
            t.RemoveDependency("e", "b");
            t.RemoveDependency("x", "y");

            IEnumerator<string> e = t.GetDependees("a").GetEnumerator();
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependees("b").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            String s1 = e.Current;
            Assert.IsTrue(e.MoveNext());
            String s2 = e.Current;
            Assert.IsFalse(e.MoveNext());
            Assert.IsTrue(((s1 == "a") && (s2 == "c")) || ((s1 == "c") && (s2 == "a")));

            e = t.GetDependees("c").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("a", e.Current);
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependees("d").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("b", e.Current);
            Assert.IsFalse(e.MoveNext());
        }

        /// <summary>
        ///Non-empty graph contains something
        ///</summary>
        [TestMethod()]
        public void NonEmptyTest15()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "y");
            t.AddDependency("a", "b");
            t.AddDependency("a", "c");
            t.AddDependency("a", "d");
            t.AddDependency("c", "b");
            t.RemoveDependency("a", "d");
            t.AddDependency("e", "b");
            t.AddDependency("b", "d");
            t.RemoveDependency("e", "b");
            t.RemoveDependency("x", "y");

            IEnumerator<string> e = t.GetDependents("a").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            String s1 = e.Current;
            Assert.IsTrue(e.MoveNext());
            String s2 = e.Current;
            Assert.IsFalse(e.MoveNext());
            Assert.IsTrue(((s1 == "b") && (s2 == "c")) || ((s1 == "c") && (s2 == "b")));

            e = t.GetDependents("b").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("d", e.Current);
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependents("c").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("b", e.Current);
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependents("d").GetEnumerator();
            Assert.IsFalse(e.MoveNext());
        }

        /// <summary>
        ///Non-empty graph contains something
        ///</summary>
        [TestMethod()]
        public void NonEmptyTest16()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "b");
            t.AddDependency("a", "z");
            t.ReplaceDependents("b", new HashSet<string>());
            t.AddDependency("y", "b");
            t.ReplaceDependents("a", new HashSet<string>() { "c" });
            t.AddDependency("w", "d");
            t.ReplaceDependees("b", new HashSet<string>() { "a", "c" });
            t.ReplaceDependees("d", new HashSet<string>() { "b" });
            Assert.AreEqual(4, t.Size);
        }

        /// <summary>
        ///Non-empty graph contains something
        ///</summary>
        [TestMethod()]
        public void NonEmptyTest18()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "b");
            t.AddDependency("a", "z");
            t.ReplaceDependents("b", new HashSet<string>());
            t.AddDependency("y", "b");
            t.ReplaceDependents("a", new HashSet<string>() { "c" });
            t.AddDependency("w", "d");
            t.ReplaceDependees("b", new HashSet<string>() { "a", "c" });
            t.ReplaceDependees("d", new HashSet<string>() { "b" });
            Assert.IsTrue(t.HasDependents("a"));
            Assert.IsFalse(t.HasDependees("a"));
            Assert.IsTrue(t.HasDependents("b"));
            Assert.IsTrue(t.HasDependees("b"));
        }

        /// <summary>
        ///Non-empty graph contains something
        ///</summary>
        [TestMethod()]
        public void NonEmptyTest19()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "b");
            t.AddDependency("a", "z");
            t.ReplaceDependents("b", new HashSet<string>());
            t.AddDependency("y", "b");
            t.ReplaceDependents("a", new HashSet<string>() { "c" });
            t.AddDependency("w", "d");
            t.ReplaceDependees("b", new HashSet<string>() { "a", "c" });
            t.ReplaceDependees("d", new HashSet<string>() { "b" });

            IEnumerator<string> e = t.GetDependees("a").GetEnumerator();
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependees("b").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            String s1 = e.Current;
            Assert.IsTrue(e.MoveNext());
            String s2 = e.Current;
            Assert.IsFalse(e.MoveNext());
            Assert.IsTrue(((s1 == "a") && (s2 == "c")) || ((s1 == "c") && (s2 == "a")));

            e = t.GetDependees("c").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("a", e.Current);
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependees("d").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("b", e.Current);
            Assert.IsFalse(e.MoveNext());
        }

        /// <summary>
        ///Non-empty graph contains something
        ///</summary>
        [TestMethod()]
        public void NonEmptyTest20()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "b");
            t.AddDependency("a", "z");
            t.ReplaceDependents("b", new HashSet<string>());
            t.AddDependency("y", "b");
            t.ReplaceDependents("a", new HashSet<string>() { "c" });
            t.AddDependency("w", "d");
            t.ReplaceDependees("b", new HashSet<string>() { "a", "c" });
            t.ReplaceDependees("d", new HashSet<string>() { "b" });

            IEnumerator<string> e = t.GetDependents("a").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            String s1 = e.Current;
            Assert.IsTrue(e.MoveNext());
            String s2 = e.Current;
            Assert.IsFalse(e.MoveNext());
            Assert.IsTrue(((s1 == "b") && (s2 == "c")) || ((s1 == "c") && (s2 == "b")));

            e = t.GetDependents("b").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("d", e.Current);
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependents("c").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("b", e.Current);
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependents("d").GetEnumerator();
            Assert.IsFalse(e.MoveNext());
        }


        // ************************** STRESS TESTS REPEATED MULTIPLE TIMES ******************************** //
        /// <summary>
        ///Using lots of data
        ///</summary>
        [TestMethod()]
        public void StressTest1()
        {
            // Dependency graph
            DependencyGraph t = new DependencyGraph();

            // A bunch of strings to use
            const int SIZE = 200;
            string[] letters = new string[SIZE];
            for (int i = 0; i < SIZE; i++)
            {
                letters[i] = ("" + (char)('a' + i));
            }

            // The correct answers
            HashSet<string>[] dents = new HashSet<string>[SIZE];
            HashSet<string>[] dees = new HashSet<string>[SIZE];
            for (int i = 0; i < SIZE; i++)
            {
                dents[i] = new HashSet<string>();
                dees[i] = new HashSet<string>();
            }

            // Add a bunch of dependencies
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = i + 1; j < SIZE; j++)
                {
                    t.AddDependency(letters[i], letters[j]);
                    dents[i].Add(letters[j]);
                    dees[j].Add(letters[i]);
                }
            }

            // Remove a bunch of dependencies
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = i + 4; j < SIZE; j += 4)
                {
                    t.RemoveDependency(letters[i], letters[j]);
                    dents[i].Remove(letters[j]);
                    dees[j].Remove(letters[i]);
                }
            }

            // Add some back
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = i + 1; j < SIZE; j += 2)
                {
                    t.AddDependency(letters[i], letters[j]);
                    dents[i].Add(letters[j]);
                    dees[j].Add(letters[i]);
                }
            }

            // Remove some more
            for (int i = 0; i < SIZE; i += 2)
            {
                for (int j = i + 3; j < SIZE; j += 3)
                {
                    t.RemoveDependency(letters[i], letters[j]);
                    dents[i].Remove(letters[j]);
                    dees[j].Remove(letters[i]);
                }
            }

            // Make sure everything is right
            for (int i = 0; i < SIZE; i++)
            {
                Assert.IsTrue(dents[i].SetEquals(new HashSet<string>(t.GetDependents(letters[i]))));
                Assert.IsTrue(dees[i].SetEquals(new HashSet<string>(t.GetDependees(letters[i]))));
            }
        }

        [TestMethod()]
        public void StressTest2()
        {
            StressTest1();
        }
        [TestMethod()]
        public void StressTest3()
        {
            StressTest1();
        }
        [TestMethod()]
        public void StressTest4()
        {
            StressTest1();
        }
        [TestMethod()]
        public void StressTest5()
        {
            StressTest1();
        }


        // ********************************** ANOTHER STESS TEST, REPEATED ******************** //
        /// <summary>
        ///Using lots of data with replacement
        ///</summary>
        [TestMethod()]
        public void StressTest8()
        {
            // Dependency graph
            DependencyGraph t = new DependencyGraph();

            // A bunch of strings to use
            const int SIZE = 400;
            string[] letters = new string[SIZE];
            for (int i = 0; i < SIZE; i++)
            {
                letters[i] = ("" + (char)('a' + i));
            }

            // The correct answers
            HashSet<string>[] dents = new HashSet<string>[SIZE];
            HashSet<string>[] dees = new HashSet<string>[SIZE];
            for (int i = 0; i < SIZE; i++)
            {
                dents[i] = new HashSet<string>();
                dees[i] = new HashSet<string>();
            }

            // Add a bunch of dependencies
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = i + 1; j < SIZE; j++)
                {
                    t.AddDependency(letters[i], letters[j]);
                    dents[i].Add(letters[j]);
                    dees[j].Add(letters[i]);
                }
            }

            // Remove a bunch of dependencies
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = i + 2; j < SIZE; j += 3)
                {
                    t.RemoveDependency(letters[i], letters[j]);
                    dents[i].Remove(letters[j]);
                    dees[j].Remove(letters[i]);
                }
            }

            // Replace a bunch of dependents
            for (int i = 0; i < SIZE; i += 2)
            {
                HashSet<string> newDents = new HashSet<String>();
                for (int j = 0; j < SIZE; j += 5)
                {
                    newDents.Add(letters[j]);
                }
                t.ReplaceDependents(letters[i], newDents);

                foreach (string s in dents[i])
                {
                    dees[s[0] - 'a'].Remove(letters[i]);
                }

                foreach (string s in newDents)
                {
                    dees[s[0] - 'a'].Add(letters[i]);
                }

                dents[i] = newDents;
            }

            // Make sure everything is right
            for (int i = 0; i < SIZE; i++)
            {
                Assert.IsTrue(dents[i].SetEquals(new HashSet<string>(t.GetDependents(letters[i]))));
                Assert.IsTrue(dees[i].SetEquals(new HashSet<string>(t.GetDependees(letters[i]))));
            }
        }

        [TestMethod()]
        public void StressTest9()
        {
            StressTest8();
        }
        [TestMethod()]
        public void StressTest10()
        {
            StressTest8();
        }
        [TestMethod()]
        public void StressTest11()
        {
            StressTest8();
        }
        [TestMethod()]
        public void StressTest12()
        {
            StressTest8();
        }


        // ********************************** A THIRD STESS TEST, REPEATED ******************** //
        /// <summary>
        ///Using lots of data with replacement
        ///</summary>
        [TestMethod()]
        public void StressTest15()
        {
            // Dependency graph
            DependencyGraph t = new DependencyGraph();

            // A bunch of strings to use
            const int SIZE = 800;
            string[] letters = new string[SIZE];
            for (int i = 0; i < SIZE; i++)
            {
                letters[i] = ("" + (char)('a' + i));
            }

            // The correct answers
            HashSet<string>[] dents = new HashSet<string>[SIZE];
            HashSet<string>[] dees = new HashSet<string>[SIZE];
            for (int i = 0; i < SIZE; i++)
            {
                dents[i] = new HashSet<string>();
                dees[i] = new HashSet<string>();
            }

            // Add a bunch of dependencies
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = i + 1; j < SIZE; j++)
                {
                    t.AddDependency(letters[i], letters[j]);
                    dents[i].Add(letters[j]);
                    dees[j].Add(letters[i]);
                }
            }

            // Remove a bunch of dependencies
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = i + 2; j < SIZE; j += 3)
                {
                    t.RemoveDependency(letters[i], letters[j]);
                    dents[i].Remove(letters[j]);
                    dees[j].Remove(letters[i]);
                }
            }

            // Replace a bunch of dependees
            for (int i = 0; i < SIZE; i += 2)
            {
                HashSet<string> newDees = new HashSet<String>();
                for (int j = 0; j < SIZE; j += 9)
                {
                    newDees.Add(letters[j]);
                }
                t.ReplaceDependees(letters[i], newDees);

                foreach (string s in dees[i])
                {
                    dents[s[0] - 'a'].Remove(letters[i]);
                }

                foreach (string s in newDees)
                {
                    dents[s[0] - 'a'].Add(letters[i]);
                }

                dees[i] = newDees;
            }

            // Make sure everything is right
            for (int i = 0; i < SIZE; i++)
            {
                Assert.IsTrue(dents[i].SetEquals(new HashSet<string>(t.GetDependents(letters[i]))));
                Assert.IsTrue(dees[i].SetEquals(new HashSet<string>(t.GetDependees(letters[i]))));
            }
        }

        [TestMethod()]
        public void StressTest16()
        {
            StressTest15();
        }
        [TestMethod()]
        public void StressTest17()
        {
            StressTest15();
        }
        [TestMethod()]
        public void StressTest18()
        {
            StressTest15();
        }
        [TestMethod()]
        public void StressTest19()
        {
            StressTest15();
        }

        //THESE ARE THE PROFESSOR'S PS4b TESTS CASES------------------------------------------------------------------------------------------------------------

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Null1()
        {
            DependencyGraph d = new DependencyGraph();
            d.AddDependency("a", null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Null2()
        {
            DependencyGraph d = new DependencyGraph();
            d.HasDependees(null);
        }

        [TestMethod]
        public void Copy1()
        {
            var d1 = new DependencyGraph();
            var d2 = new DependencyGraph(d1);
            Assert.AreEqual(0, d1.Size);
            Assert.AreEqual(0, d2.Size);
        }

        [TestMethod]
        public void Copy2()
        {
            var d1 = new DependencyGraph();
            var d2 = new DependencyGraph(d1);
            d1.AddDependency("a", "b");
            Assert.AreEqual(1, d1.Size);
            Assert.AreEqual(0, d2.Size);
        }

        [TestMethod]
        public void Copy3()
        {
            var d1 = new DependencyGraph();
            var d2 = new DependencyGraph(d1);
            d1.AddDependency("a", "b");
            d2.AddDependency("c", "d");
            Assert.IsTrue(d1.HasDependents("a"));
            Assert.IsFalse(d1.HasDependents("c"));
            Assert.IsFalse(d2.HasDependents("a"));
            Assert.IsTrue(d2.HasDependents("c"));
        }

        [TestMethod]
        public void Copy4()
        {
            var d1 = new DependencyGraph();
            d1.AddDependency("a", "b");
            var d2 = new DependencyGraph(d1);
            Assert.IsTrue(d1.HasDependees("b"));
            Assert.IsTrue(d2.HasDependees("b"));
        }

        [TestMethod]
        public void Copy5()
        {
            var d1 = new DependencyGraph();
            d1.AddDependency("a", "b");
            d1.AddDependency("d", "e");
            var d2 = new DependencyGraph(d1);
            d1.AddDependency("a", "c");
            d2.AddDependency("d", "f");
            Assert.AreEqual(2, new List<string>(d1.GetDependents("a")).Count);
            Assert.AreEqual(1, new List<string>(d1.GetDependents("d")).Count);
            Assert.AreEqual(2, new List<string>(d2.GetDependents("d")).Count);
            Assert.AreEqual(1, new List<string>(d2.GetDependents("a")).Count);
        }

        [TestMethod]
        public void Copy6()
        {
            var d1 = new DependencyGraph();
            d1.AddDependency("b", "a");
            d1.AddDependency("e", "d");
            var d2 = new DependencyGraph(d1);
            d1.AddDependency("c", "a");
            d2.AddDependency("f", "d");
            Assert.AreEqual(2, new List<string>(d1.GetDependees("a")).Count);
            Assert.AreEqual(1, new List<string>(d1.GetDependees("d")).Count);
            Assert.AreEqual(2, new List<string>(d2.GetDependees("d")).Count);
            Assert.AreEqual(1, new List<string>(d2.GetDependees("a")).Count);
        }

        [TestMethod]
        public void Copy7()
        {
            var d1 = new DependencyGraph();
            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j <= i; j++)
                {
                    d1.AddDependency(i.ToString(), j.ToString());
                }
            }
            var d2 = new DependencyGraph(d1);

            for (int i = 0; i < 100; i++)
            {
                d1.RemoveDependency(i.ToString(), i.ToString());
                d2.AddDependency(i.ToString(), "x");
            }

            for (int i = 0; i < 100; i++)
            {
                Assert.AreEqual(i, new List<string>(d1.GetDependents(i.ToString())).Count);
                Assert.AreEqual(i + 2, new List<string>(d2.GetDependents(i.ToString())).Count);
            }

            for (int j = 0; j <= 50; j++)
            {
                Assert.AreEqual(99 - j, new List<string>(d1.GetDependees(j.ToString())).Count);
                Assert.AreEqual(100 - j, new List<string>(d2.GetDependees(j.ToString())).Count);
            }

            Assert.AreEqual(100, new List<string>(d2.GetDependees("x")).Count);

            Assert.AreEqual(5050 - 100, d1.Size);
            Assert.AreEqual(5050 + 100, d2.Size);
        }

        [TestMethod]
        public void Copy8()
        {
            Copy7();
        }
    }
}
