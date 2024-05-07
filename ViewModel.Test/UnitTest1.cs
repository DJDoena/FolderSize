using DoenaSoft.FolderSize.ViewModel;
using DoenaSoft.FolderSize.ViewModel.Test;

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
    }

    [TestMethod]
    public void TestMethod2()
    {
        var testFolderNode = new TestFolderNode();

        using var cts = new CancellationTokenSource();

        var vm = new NodeViewModel(testFolderNode, testFolderNode.Parent, cts.Token);

        var children = vm.Children.ToList();

        Assert.AreEqual(1, children.Count);
    }
}