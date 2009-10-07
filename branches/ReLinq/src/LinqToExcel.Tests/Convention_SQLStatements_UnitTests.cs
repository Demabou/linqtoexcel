﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MbUnit.Framework;
using log4net;
using System.Reflection;
using log4net.Appender;
using log4net.Core;
using System.Data.OleDb;

namespace LinqToExcel.Tests
{
    [Author("Paul Yoder", "paulyoder@gmail.com")]
    [FixtureCategory("Unit")]
    [TestFixture]
    public class Convention_SQLStatements_UnitTests : SQLLogStatements_Helper
    {
        [TestFixtureSetUp]
        public void fs()
        {
            InstantiateLogger();
        }

        [SetUp]
        public void s()
        {
            ClearLogEvents();
        }

        [Test]
        public void select_all()
        {
            var companies = from c in ExcelQueryFactory.Worksheet<Company>("")
                            select c;

            try { companies.GetEnumerator(); }
            catch (OleDbException) { }
            Assert.AreEqual("SELECT * FROM [Sheet1$]", GetSQLStatement());
        }

        [Test]
        public void where_equals()
        {
            var companies = from p in ExcelQueryFactory.Worksheet<Company>("")
                            where p.Name == "Paul"
                            select p;

            try { companies.GetEnumerator(); }
            catch (OleDbException) { }
            var expectedSql = string.Format("SELECT * FROM [Sheet1$] WHERE ({0} = ?)", GetSQLFieldName("Name"));
            Assert.AreEqual(expectedSql, GetSQLStatement());
            Assert.AreEqual("Paul", GetSQLParameters()[0]);
        }

        [Test]
        public void where_not_equal()
        {
            var companies = from p in ExcelQueryFactory.Worksheet<Company>("")
                            where p.Name != "Paul"
                            select p;

            try { companies.GetEnumerator(); }
            catch (OleDbException) { }
            var expectedSql = string.Format("SELECT * FROM [Sheet1$] WHERE ({0} <> ?)", GetSQLFieldName("Name"));
            Assert.AreEqual(expectedSql, GetSQLStatement());
            Assert.AreEqual("Paul", GetSQLParameters()[0]);
        }

        [Test]
        public void where_greater_than()
        {
            var companies = from p in ExcelQueryFactory.Worksheet<Company>("")
                            where p.EmployeeCount > 25
                            select p;

            try { companies.GetEnumerator(); }
            catch (OleDbException) { }
            var expectedSql = string.Format("SELECT * FROM [Sheet1$] WHERE ({0} > ?)", GetSQLFieldName("EmployeeCount"));
            Assert.AreEqual(expectedSql, GetSQLStatement());
            Assert.AreEqual("25", GetSQLParameters()[0]);
        }

        [Test]
        public void where_greater_than_or_equal()
        {
            var companies = from p in ExcelQueryFactory.Worksheet<Company>("")
                            where p.EmployeeCount >= 25
                            select p;

            try { companies.GetEnumerator(); }
            catch (OleDbException) { }
            var expectedSql = string.Format("SELECT * FROM [Sheet1$] WHERE ({0} >= ?)", GetSQLFieldName("EmployeeCount"));
            Assert.AreEqual(expectedSql, GetSQLStatement());
            Assert.AreEqual("25", GetSQLParameters()[0]);
        }

        [Test]
        public void where_lesser_than()
        {
            var companies = from p in ExcelQueryFactory.Worksheet<Company>("")
                            where p.EmployeeCount < 25
                            select p;

            try { companies.GetEnumerator(); }
            catch (OleDbException) { }
            var expectedSql = string.Format("SELECT * FROM [Sheet1$] WHERE ({0} < ?)", GetSQLFieldName("EmployeeCount"));
            Assert.AreEqual(expectedSql, GetSQLStatement());
            Assert.AreEqual("25", GetSQLParameters()[0]);
        }

        [Test]
        public void where_lesser_than_or_equal()
        {
            var companies = from p in ExcelQueryFactory.Worksheet<Company>("")
                            where p.EmployeeCount <= 25
                            select p;

            try { companies.GetEnumerator(); }
            catch (OleDbException) { }
            var expectedSql = string.Format("SELECT * FROM [Sheet1$] WHERE ({0} <= ?)", GetSQLFieldName("EmployeeCount"));
            Assert.AreEqual(expectedSql, GetSQLStatement());
            Assert.AreEqual("25", GetSQLParameters()[0]);
        }

        [Test]
        public void where_and()
        {
            var companies = from p in ExcelQueryFactory.Worksheet<Company>("")
                            where p.EmployeeCount > 5 && p.CEO == "Paul"
                            select p;

            try { companies.GetEnumerator(); }
            catch (OleDbException) { }
            var expectedSql = string.Format("SELECT * FROM [Sheet1$] WHERE (({0} > ?) AND ({1} = ?))",
                                                GetSQLFieldName("EmployeeCount"),
                                                GetSQLFieldName("CEO"));
            var parameters = GetSQLParameters();

            Assert.AreEqual(expectedSql, GetSQLStatement());
            Assert.AreEqual("5", parameters[0]);
            Assert.AreEqual("Paul", parameters[1]);
        }

        [Test]
        public void where_or()
        {
            var companies = from p in ExcelQueryFactory.Worksheet<Company>("")
                            where p.EmployeeCount > 5 || p.CEO == "Paul"
                            select p;

            try { companies.GetEnumerator(); }
            catch (OleDbException) { }
            var expectedSql = string.Format("SELECT * FROM [Sheet1$] WHERE (({0} > ?) OR ({1} = ?))",
                                                GetSQLFieldName("EmployeeCount"),
                                                GetSQLFieldName("CEO"));
            var parameters = GetSQLParameters();

            Assert.AreEqual(expectedSql, GetSQLStatement());
            Assert.AreEqual("5", parameters[0]);
            Assert.AreEqual("Paul", parameters[1]);
        }

        private string GetName(string name)
        {
            return name;
        }

        [Test]
        public void method_used_in_where_clause()
        {
            var companies = from p in ExcelQueryFactory.Worksheet<Company>("")
                            where p.Name == GetName("Paul")
                            select p;

            try { companies.GetEnumerator(); }
            catch (OleDbException) { }
            Assert.AreEqual("Paul", GetSQLParameters()[0]);
        }

        [Test]
        public void where_contains()
        {
            var companies = from p in ExcelQueryFactory.Worksheet<Company>("")
                            where p.Name.Contains("Paul")
                            select p;
            
            try { companies.GetEnumerator(); }
            catch (OleDbException) { }
            var expectedSql = string.Format("SELECT * FROM [Sheet1$] WHERE ({0} LIKE ?)", GetSQLFieldName("Name"));
            Assert.AreEqual(expectedSql, GetSQLStatement());
            Assert.AreEqual("%Paul%", GetSQLParameters()[0]);
        }

        [Test]
        public void first()
        {
            var companies = from c in ExcelQueryFactory.Worksheet<Company>("")
                            select c;
            
            try { companies.First(); }
            catch (OleDbException) { }
            Assert.AreEqual("SELECT TOP 1 * FROM [Sheet1$]", GetSQLStatement());
        }
    }
}
