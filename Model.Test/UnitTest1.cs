namespace DoenaSoft.FolderSize.Model.Test;

[TestClass]
public class UnitTest1
{
    [ClassInitialize]
    public static void ClassInitialize(TestContext testContext)
    {
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
    }

    [TestInitialize]
    public void TestInitialize()
    {
    }

    [TestCleanup]
    public void TestCleanup()
    {
    }

    [TestMethod]
    public void TestMethod1()
    {
        var folderInfo = new TestFolderInfo();

        var folderNode = new FolderNode(folderInfo);

        var children = folderNode.Children.ToList();

        Assert.AreEqual(2, children.Count);
    }

    [TestMethod]
    public void TestMethod2()
    {
    }
}