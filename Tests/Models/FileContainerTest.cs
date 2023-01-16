using System.Collections.ObjectModel;
using Filer.Models;
using NUnit.Framework;

namespace Tests.Models
{
    [TestFixture]
    public class FileContainerTest
    {
        [Test]
        public void DownCursorTest()
        {
            var fileContainer = new FileContainer
            {
                Files = new ObservableCollection<ExtendFileInfo>()
                {
                    new ExtendFileInfo("a"),
                    new ExtendFileInfo("b"),
                }
            };

            Assert.AreEqual(0, fileContainer.SelectedIndex, "初期値は 0");

            fileContainer.DownCursor(1);
            Assert.AreEqual(1, fileContainer.SelectedIndex, "index+1 = 1 ");

            fileContainer.DownCursor(1);
            Assert.AreEqual(1, fileContainer.SelectedIndex, "最後尾なのでインデックスは増えない");

            fileContainer.SelectedIndex = 0;
            fileContainer.DownCursor(4);
            Assert.AreEqual(1, fileContainer.SelectedIndex, "過剰な数を投入しても問題ないか？");
        }

        [Test]
        public void UpCursorTest()
        {
            var fileContainer = new FileContainer
            {
                Files = new ObservableCollection<ExtendFileInfo>()
                {
                    new ExtendFileInfo("a"),
                    new ExtendFileInfo("b"),
                }
            };

            fileContainer.SelectedIndex = 1;

            fileContainer.UpCursor(1);
            Assert.AreEqual(0, fileContainer.SelectedIndex, "index-1 = 0 ");

            fileContainer.UpCursor(1);
            Assert.AreEqual(0, fileContainer.SelectedIndex, "既に先頭なのでインデックスは減らない");

            fileContainer.SelectedIndex = 1;
            fileContainer.UpCursor(4);
            Assert.AreEqual(0, fileContainer.SelectedIndex, "過剰な数を投入しても問題ないか？");
        }

        [Test]
        public void JumpToLastTest()
        {
            var fileContainer = new FileContainer
            {
                Files = new ObservableCollection<ExtendFileInfo>()
                {
                    new ExtendFileInfo("a"),
                    new ExtendFileInfo("b"),
                    new ExtendFileInfo("c"),
                }
            };

            Assert.AreEqual(0, fileContainer.SelectedIndex);

            fileContainer.JumpToLast();
            Assert.AreEqual(2, fileContainer.SelectedIndex);
        }

        [Test]
        public void JumpToHeadTest()
        {
            var fileContainer = new FileContainer
            {
                Files = new ObservableCollection<ExtendFileInfo>()
                {
                    new ExtendFileInfo("a"),
                    new ExtendFileInfo("b"),
                    new ExtendFileInfo("c"),
                },
                SelectedIndex = 2
            };

            fileContainer.JumpToHead();
            Assert.AreEqual(0, fileContainer.SelectedIndex);
        }
    }
}