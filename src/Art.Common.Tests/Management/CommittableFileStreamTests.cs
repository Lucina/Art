using System;
using System.IO;
using System.Text;
using Art.Common.Management;
using NUnit.Framework;

namespace Art.Common.Tests.Management;

public class CommittableFileStreamTests
{
    [Test]
    public void ShouldCommit_TrueWithNewFile_FileKeptWithContents()
    {
        string tempDir = Path.GetTempPath();
        Assert.That(Directory.Exists(tempDir), Is.True);
        string temp = CommittableFileStream.CreateRandomPath(tempDir);
        Assert.That(Path.GetRelativePath(tempDir, temp), Is.EqualTo(Path.GetFileName(temp)));
        try
        {
            string mess = $"ya like jazz? {Random.Shared.Next()}";
            byte[] data = Encoding.UTF8.GetBytes(mess);
            using (CommittableFileStream cfs = new(temp, FileMode.Create))
            {
                cfs.Write(data);
                cfs.ShouldCommit = true;
            }
            Assert.That(File.Exists(temp), Is.True);
            Assert.That(File.ReadAllBytes(temp).AsSpan().SequenceEqual(data), Is.True);
        }
        finally
        {
            File.Delete(temp);
        }
    }

    [Test]
    public void ShouldCommit_FalseWithNewFile_FileNotExist()
    {
        string tempDir = Path.GetTempPath();
        Assert.That(Directory.Exists(tempDir), Is.True);
        string temp = CommittableFileStream.CreateRandomPath(tempDir);
        Assert.That(Path.GetRelativePath(tempDir, temp), Is.EqualTo(Path.GetFileName(temp)));
        try
        {
            string mess = $"ya like jazz? {Random.Shared.Next()}";
            byte[] data = Encoding.UTF8.GetBytes(mess);
            using (CommittableFileStream cfs = new(temp, FileMode.Create))
            {
                cfs.Write(data);
                //cfs.ShouldCommit = false;
            }
            Assert.That(File.Exists(temp), Is.False);
        }
        finally
        {
            File.Delete(temp);
        }
    }

    [Test]
    public void ShouldCommit_TrueWithExisting_NewFileKeptWithContents()
    {
        string temp = Path.GetTempFileName();
        try
        {
            string mess0 = $"ya like jazz? {Random.Shared.Next()}";
            byte[] data0 = Encoding.UTF8.GetBytes(mess0);
            File.WriteAllBytes(temp, data0);
            Assert.That(File.Exists(temp), Is.True);
            string mess1 = $"ya like jazz? {Random.Shared.Next()}";
            byte[] data1 = Encoding.UTF8.GetBytes(mess1);
            using (CommittableFileStream cfs = new(temp, FileMode.Create))
            {
                cfs.Write(data1);
                cfs.ShouldCommit = true;
            }
            Assert.That(File.Exists(temp), Is.True);
            Assert.That(File.ReadAllBytes(temp).AsSpan().SequenceEqual(data1), Is.True);
        }
        finally
        {
            File.Delete(temp);
        }
    }

    [Test]
    public void ShouldCommit_FalseWithExisting_OldFileKeptWithContents()
    {
        string temp = Path.GetTempFileName();
        try
        {
            string mess0 = $"ya like jazz? {Random.Shared.Next()}";
            byte[] data0 = Encoding.UTF8.GetBytes(mess0);
            File.WriteAllBytes(temp, data0);
            Assert.That(File.Exists(temp), Is.True);
            string mess1 = $"ya like jazz? {Random.Shared.Next()}";
            byte[] data1 = Encoding.UTF8.GetBytes(mess1);
            using (CommittableFileStream cfs = new(temp, FileMode.Create))
            {
                cfs.Write(data1);
                //cfs.ShouldCommit = false;
            }
            Assert.That(File.Exists(temp), Is.True);
            Assert.That(File.ReadAllBytes(temp).AsSpan().SequenceEqual(data0), Is.True);
        }
        finally
        {
            File.Delete(temp);
        }
    }
}
