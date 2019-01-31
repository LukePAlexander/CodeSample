using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Windows.Forms;
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UITest.Extension;
using Keyboard = Microsoft.VisualStudio.TestTools.UITesting.Keyboard;


namespace CodedUITestProject2
{
    /// <summary>
    /// Summary description for CodedUITest1
    /// </summary>
    [CodedUITest]
    public class CodedUITest1
    {
        public CodedUITest1()
        {
        }

        [TestMethod]
        public void CodedUITestMethod1()
        {
            // To generate code for this test, select "Generate Code for Coded UI Test" from the shortcut menu and select one of the menu items.
            this.UIMap.TestEntryAndNav1();
            this.UIMap.AssertA1();
            this.UIMap.TestEntryAndNav2();
            this.UIMap.AssertA2();
            this.UIMap.TestEntryAndNav3();
            this.UIMap.AssertPositionIsA2();
            this.UIMap.TestEntryAndNav4();
            this.UIMap.AssertA3Formula();
            this.UIMap.TestEntryAndNav5();
            this.UIMap.AssertB2String();
            this.UIMap.TestEntryAndNavSave1();
            this.UIMap.TestSaveDialog1();
            this.UIMap.TestClose1();
            this.UIMap.Test1();
            this.UIMap.Assert1();
            this.UIMap.Test2();
            this.UIMap.Assert2();
            this.UIMap.Test3();
            this.UIMap.Assert3();
            this.UIMap.Test4();
            this.UIMap.Test4B();
            this.UIMap.Assert4();
            this.UIMap.Test5();
            this.UIMap.Assert5();
            this.UIMap.Test6();
            this.UIMap.Assert6();
            this.UIMap.Test7();
            this.UIMap.Assert7();
            this.UIMap.Test8();
            this.UIMap.AssertMethod1();
            this.UIMap.Test9A();
            this.UIMap.Assert9A();
            this.UIMap.Assert9B();
            this.UIMap.Assert9C();
            this.UIMap.Test10();
            this.UIMap.Test10B();
            this.UIMap.Assert10();
            this.UIMap.Test11();
            this.UIMap.Assert10B();
            this.UIMap.Test12();
            this.UIMap.Assert11();
            this.UIMap.Test12B();
            this.UIMap.Assert11B();
            this.UIMap.Test13();
            this.UIMap.Test13B();
            this.UIMap.AssertMethod2();
            this.UIMap.Assert13();
            this.UIMap.Assert13B();
            this.UIMap.Test14();
            this.UIMap.Assert14();
            this.UIMap.Assert14B();
            this.UIMap.Assert14C();
            this.UIMap.Test15();
            this.UIMap.Assert15();
            this.UIMap.Test16();
            this.UIMap.Test16B();
            this.UIMap.Assert16();
            this.UIMap.Assert16B();
            this.UIMap.Test17();
            this.UIMap.Test18();
            this.UIMap.Test19();
            this.UIMap.Test19A();
            this.UIMap.Assert17();
            this.UIMap.Test20();
            this.UIMap.Assert18();
            this.UIMap.Test21();
            this.UIMap.Assert19();
            this.UIMap.Test22();
        }

        #region Additional test attributes

        // You can use the following additional attributes as you write your tests:

        ////Use TestInitialize to run code before running each test 
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{        
        //    // To generate code for this test, select "Generate Code for Coded UI Test" from the shortcut menu and select one of the menu items.
        //}

        ////Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{        
        //    // To generate code for this test, select "Generate Code for Coded UI Test" from the shortcut menu and select one of the menu items.
        //}

        #endregion

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }
        private TestContext testContextInstance;

        public UIMap UIMap
        {
            get
            {
                if ((this.map == null))
                {
                    this.map = new UIMap();
                }

                return this.map;
            }
        }

        private UIMap map;
    }
}
