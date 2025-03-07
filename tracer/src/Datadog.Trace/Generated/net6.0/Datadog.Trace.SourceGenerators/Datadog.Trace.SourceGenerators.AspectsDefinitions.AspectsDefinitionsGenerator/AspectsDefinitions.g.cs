﻿// <auto-generated/>
#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Datadog.Trace.ClrProfiler
{
    internal static partial class AspectDefinitions
    {
        public static string[] Aspects = new string[] {
"[AspectClass(\"mscorlib,System.IO.FileSystem,System.Runtime\",[None],Sink,[PathTraversal])] Datadog.Trace.Iast.Aspects.DirectoryAspect",
"  [AspectMethodInsertBefore(\"System.IO.Directory::CreateDirectory(System.String)\",\"\",[0],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.Directory::CreateDirectory(System.String,System.IO.UnixFileMode)\",\"\",[1],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.Directory::CreateTempSubdirectory(System.String)\",\"\",[0],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.Directory::Delete(System.String)\",\"\",[0],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.Directory::Delete(System.String,System.Boolean)\",\"\",[1],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.Directory::GetDirectories(System.String)\",\"\",[0],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.Directory::GetDirectories(System.String,System.String)\",\"\",[0,1],[False,False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.Directory::GetDirectories(System.String,System.String,System.IO.SearchOption)\",\"\",[1,2],[False,False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.Directory::GetDirectories(System.String,System.String,System.IO.EnumerationOptions)\",\"\",[1,2],[False,False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.Directory::GetDirectoryRoot(System.String)\",\"\",[0],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.Directory::GetFiles(System.String)\",\"\",[0],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.Directory::GetFiles(System.String,System.String)\",\"\",[0,1],[False,False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.Directory::GetFiles(System.String,System.String,System.IO.SearchOption)\",\"\",[1,2],[False,False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.Directory::GetFiles(System.String,System.String,System.IO.EnumerationOptions)\",\"\",[1,2],[False,False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.Directory::GetFileSystemEntries(System.String)\",\"\",[0],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.Directory::GetFileSystemEntries(System.String,System.String)\",\"\",[0,1],[False,False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.Directory::GetFileSystemEntries(System.String,System.String,System.IO.SearchOption)\",\"\",[1,2],[False,False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.Directory::GetFileSystemEntries(System.String,System.String,System.IO.EnumerationOptions)\",\"\",[1,2],[False,False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.Directory::Move(System.String,System.String)\",\"\",[0,1],[False,False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.Directory::SetAccessControl(System.String,System.Security.AccessControl.DirectorySecurity)\",\"\",[1],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.Directory::EnumerateDirectories(System.String)\",\"\",[0],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.Directory::EnumerateDirectories(System.String,System.String,System.IO.EnumerationOptions)\",\"\",[1,2],[False,False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.Directory::EnumerateDirectories(System.String,System.String)\",\"\",[0,1],[False,False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.Directory::EnumerateDirectories(System.String,System.String,System.IO.SearchOption)\",\"\",[1,2],[False,False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.Directory::EnumerateFiles(System.String)\",\"\",[0],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.Directory::EnumerateFiles(System.String,System.String,System.IO.EnumerationOptions)\",\"\",[1,2],[False,False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.Directory::EnumerateFiles(System.String,System.String)\",\"\",[0,1],[False,False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.Directory::EnumerateFiles(System.String,System.String,System.IO.SearchOption)\",\"\",[1,2],[False,False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.Directory::EnumerateFileSystemEntries(System.String)\",\"\",[0],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.Directory::EnumerateFileSystemEntries(System.String,System.String)\",\"\",[0,1],[False,False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.Directory::EnumerateFileSystemEntries(System.String,System.String,System.IO.SearchOption)\",\"\",[1,2],[False,False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.Directory::EnumerateFileSystemEntries(System.String,System.String,System.IO.EnumerationOptions)\",\"\",[1,2],[False,False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.Directory::SetCurrentDirectory(System.String)\",\"\",[0],[False],[None],Propagation,[])] ReviewPath(System.String)",
"[AspectClass(\"mscorlib,System.IO.FileSystem,System.Runtime\",[None],Sink,[PathTraversal])] Datadog.Trace.Iast.Aspects.DirectoryInfoAspect",
"  [AspectMethodInsertBefore(\"System.IO.DirectoryInfo::.ctor(System.String)\",\"\",[0],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.DirectoryInfo::CreateSubdirectory(System.String)\",\"\",[0],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.DirectoryInfo::MoveTo(System.String)\",\"\",[0],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.DirectoryInfo::GetFileSystemInfos(System.String)\",\"\",[0],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.DirectoryInfo::GetFileSystemInfos(System.String,System.IO.SearchOption)\",\"\",[1],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.DirectoryInfo::GetFileSystemInfos(System.String,System.IO.EnumerationOptions)\",\"\",[1],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.DirectoryInfo::GetFiles(System.String)\",\"\",[0],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.DirectoryInfo::GetFiles(System.String,System.IO.SearchOption)\",\"\",[1],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.DirectoryInfo::GetFiles(System.String,System.IO.EnumerationOptions)\",\"\",[1],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.DirectoryInfo::GetDirectories(System.String)\",\"\",[0],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.DirectoryInfo::GetDirectories(System.String,System.IO.SearchOption)\",\"\",[1],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.DirectoryInfo::GetDirectories(System.String,System.IO.EnumerationOptions)\",\"\",[1],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.DirectoryInfo::EnumerateFileSystemInfos(System.String)\",\"\",[0],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.DirectoryInfo::EnumerateFileSystemInfos(System.String,System.IO.SearchOption)\",\"\",[1],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.DirectoryInfo::EnumerateFileSystemInfos(System.String,System.IO.EnumerationOptions)\",\"\",[1],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.DirectoryInfo::EnumerateFiles(System.String)\",\"\",[0],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.DirectoryInfo::EnumerateFiles(System.String,System.IO.SearchOption)\",\"\",[1],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.DirectoryInfo::EnumerateFiles(System.String,System.IO.EnumerationOptions)\",\"\",[1],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.DirectoryInfo::EnumerateDirectories(System.String)\",\"\",[0],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.DirectoryInfo::EnumerateDirectories(System.String,System.IO.SearchOption)\",\"\",[1],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.DirectoryInfo::EnumerateDirectories(System.String,System.IO.EnumerationOptions)\",\"\",[1],[False],[None],Propagation,[])] ReviewPath(System.String)",
"[AspectClass(\"mscorlib,System.IO.FileSystem,System.Runtime\",[None],Sink,[PathTraversal])] Datadog.Trace.Iast.Aspects.FileAspect",
"  [AspectMethodInsertBefore(\"System.IO.File::Create(System.String)\",\"\",[0],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.File::CreateText(System.String)\",\"\",[0],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.File::Delete(System.String)\",\"\",[0],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.File::OpenRead(System.String)\",\"\",[0],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.File::OpenText(System.String)\",\"\",[0],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.File::OpenWrite(System.String)\",\"\",[0],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.File::ReadAllBytes(System.String)\",\"\",[0],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.File::ReadAllBytesAsync(System.String,System.Threading.CancellationToken)\",\"\",[1],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.File::ReadAllLines(System.String)\",\"\",[0],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.File::ReadAllLines(System.String,System.Text.Encoding)\",\"\",[1],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.File::ReadAllLinesAsync(System.String,System.Threading.CancellationToken)\",\"\",[1],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.File::ReadAllLinesAsync(System.String,System.Text.Encoding,System.Threading.CancellationToken)\",\"\",[2],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.File::ReadAllText(System.String)\",\"\",[0],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.File::ReadAllTextAsync(System.String,System.Text.Encoding,System.Threading.CancellationToken)\",\"\",[2],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.File::ReadAllTextAsync(System.String,System.Threading.CancellationToken)\",\"\",[1],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.File::ReadLines(System.String)\",\"\",[0],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.File::ReadLinesAsync(System.String,System.Threading.CancellationToken)\",\"\",[1],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.File::ReadLinesAsync(System.String,System.Text.Encoding,System.Threading.CancellationToken)\",\"\",[2],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.File::AppendAllLines(System.String,System.Collections.Generic.IEnumerable`1<System.String>)\",\"\",[1],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.File::AppendAllLines(System.String,System.Collections.Generic.IEnumerable`1<System.String>,System.Text.Encoding)\",\"\",[2],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.File::AppendAllText(System.String,System.String)\",\"\",[1],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.File::AppendAllText(System.String,System.String,System.Text.Encoding)\",\"\",[2],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.File::AppendAllTextAsync(System.String,System.String,System.Threading.CancellationToken)\",\"\",[2],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.File::AppendAllTextAsync(System.String,System.String,System.Text.Encoding,System.Threading.CancellationToken)\",\"\",[3],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.File::AppendAllLinesAsync(System.String,System.Collections.Generic.IEnumerable`1<System.String>,System.Threading.CancellationToken)\",\"\",[2],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.File::AppendAllLinesAsync(System.String,System.Collections.Generic.IEnumerable`1<System.String>,System.Text.Encoding,System.Threading.CancellationToken)\",\"\",[3],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.File::AppendText(System.String)\",\"\",[0],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.File::ReadLines(System.String,System.Text.Encoding)\",\"\",[1],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.File::ReadAllText(System.String,System.Text.Encoding)\",\"\",[1],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.File::ReadLines(System.String,System.Text.Encoding)\",\"\",[1],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.File::Create(System.String,System.Int32)\",\"\",[1],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.File::Create(System.String,System.Int32,System.IO.FileOptions)\",\"\",[2],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.File::Open(System.String,System.IO.FileMode)\",\"\",[1],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.File::Open(System.String,System.IO.FileMode,System.IO.FileAccess)\",\"\",[2],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.File::Open(System.String,System.IO.FileMode,System.IO.FileAccess,System.IO.FileShare)\",\"\",[3],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.File::Open(System.String,System.IO.FileStreamOptions)\",\"\",[1],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.File::OpenHandle(System.String,System.IO.FileMode,System.IO.FileAccess,System.IO.FileShare,System.IO.FileOptions,System.Int64)\",\"\",[5],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.File::SetAttributes(System.String,System.IO.FileAttributes)\",\"\",[1],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.File::WriteAllBytes(System.String,System.Byte[])\",\"\",[1],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.File::WriteAllBytesAsync(System.String,System.Byte[],System.Threading.CancellationToken)\",\"\",[2],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.File::WriteAllLines(System.String,System.String[])\",\"\",[1],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.File::WriteAllLines(System.String,System.String[],System.Text.Encoding)\",\"\",[2],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.File::WriteAllLines(System.String,System.Collections.Generic.IEnumerable`1<System.String>)\",\"\",[1],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.File::WriteAllLines(System.String,System.Collections.Generic.IEnumerable`1<System.String>,System.Text.Encoding)\",\"\",[2],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.File::WriteAllLinesAsync(System.String,System.Collections.Generic.IEnumerable`1<System.String>,System.Text.Encoding,System.Threading.CancellationToken)\",\"\",[3],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.File::WriteAllLinesAsync(System.String,System.Collections.Generic.IEnumerable`1<System.String>,System.Threading.CancellationToken)\",\"\",[2],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.File::WriteAllText(System.String,System.String)\",\"\",[1],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.File::WriteAllText(System.String,System.String,System.Text.Encoding)\",\"\",[2],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.File::WriteAllTextAsync(System.String,System.String,System.Threading.CancellationToken)\",\"\",[2],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.File::WriteAllTextAsync(System.String,System.String,System.Text.Encoding,System.Threading.CancellationToken)\",\"\",[3],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.File::Copy(System.String,System.String)\",\"\",[0,1],[False,False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.File::Copy(System.String,System.String,System.Boolean)\",\"\",[1,2],[False,False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.File::Move(System.String,System.String)\",\"\",[0,1],[False,False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.File::Move(System.String,System.String,System.Boolean)\",\"\",[1,2],[False,False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.File::Replace(System.String,System.String,System.String)\",\"\",[0,1,2],[False,False,False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.File::Replace(System.String,System.String,System.String,System.Boolean)\",\"\",[1,2,3],[False,False,False],[None],Propagation,[])] ReviewPath(System.String)",
"[AspectClass(\"mscorlib,System.IO.FileSystem,System.Runtime\",[None],Sink,[PathTraversal])] Datadog.Trace.Iast.Aspects.FileInfoAspect",
"  [AspectMethodInsertBefore(\"System.IO.FileInfo::.ctor(System.String)\",\"\",[0],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.FileInfo::CopyTo(System.String)\",\"\",[0],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.FileInfo::CopyTo(System.String,System.Boolean)\",\"\",[1],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.FileInfo::MoveTo(System.String,System.Boolean)\",\"\",[1],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.FileInfo::MoveTo(System.String)\",\"\",[0],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.FileInfo::Replace(System.String,System.String)\",\"\",[0,1],[False,False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.FileInfo::Replace(System.String,System.String,System.Boolean)\",\"\",[1,2],[False,False],[None],Propagation,[])] ReviewPath(System.String)",
"[AspectClass(\"mscorlib,System.IO.FileSystem,System.Runtime\",[None],Sink,[PathTraversal])] Datadog.Trace.Iast.Aspects.FileStreamAspect",
"  [AspectMethodInsertBefore(\"System.IO.FileStream::.ctor(System.String,System.IO.FileMode)\",\"\",[1],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.FileStream::.ctor(System.String,System.IO.FileMode,System.IO.FileAccess)\",\"\",[2],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.FileStream::.ctor(System.String,System.IO.FileMode,System.IO.FileAccess,System.IO.FileShare)\",\"\",[3],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.FileStream::.ctor(System.String,System.IO.FileMode,System.IO.FileAccess,System.IO.FileShare,System.Int32)\",\"\",[4],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.FileStream::.ctor(System.String,System.IO.FileMode,System.IO.FileAccess,System.IO.FileShare,System.Int32,System.Boolean)\",\"\",[5],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.FileStream::.ctor(System.String,System.IO.FileMode,System.IO.FileAccess,System.IO.FileShare,System.Int32,System.IO.FileOptions)\",\"\",[5],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.FileStream::.ctor(System.String,System.IO.FileStreamOptions)\",\"\",[1],[False],[None],Propagation,[])] ReviewPath(System.String)",
"[AspectClass(\"mscorlib,System.IO.FileSystem,System.Runtime\",[None],Sink,[PathTraversal])] Datadog.Trace.Iast.Aspects.StreamReaderAspect",
"  [AspectMethodInsertBefore(\"System.IO.StreamReader::.ctor(System.String)\",\"\",[0],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.StreamReader::.ctor(System.String,System.Boolean)\",\"\",[1],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.StreamReader::.ctor(System.String,System.Text.Encoding)\",\"\",[1],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.StreamReader::.ctor(System.String,System.Text.Encoding,System.Boolean)\",\"\",[2],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.StreamReader::.ctor(System.String,System.Text.Encoding,System.Boolean,System.Int32)\",\"\",[3],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.StreamReader::.ctor(System.String,System.Text.Encoding,System.Boolean,System.IO.FileStreamOptions)\",\"\",[3],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.StreamReader::.ctor(System.String,System.IO.FileStreamOptions)\",\"\",[1],[False],[None],Propagation,[])] ReviewPath(System.String)",
"[AspectClass(\"mscorlib,System.IO.FileSystem,System.Runtime\",[None],Sink,[PathTraversal])] Datadog.Trace.Iast.Aspects.StreamWriterAspect",
"  [AspectMethodInsertBefore(\"System.IO.StreamWriter::.ctor(System.String,System.Text.Encoding,System.IO.FileStreamOptions)\",\"\",[2],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.StreamWriter::.ctor(System.String,System.IO.FileStreamOptions)\",\"\",[1],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.StreamWriter::.ctor(System.String)\",\"\",[0],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.StreamWriter::.ctor(System.String,System.Boolean)\",\"\",[1],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.StreamWriter::.ctor(System.String,System.Boolean,System.Text.Encoding)\",\"\",[2],[False],[None],Propagation,[])] ReviewPath(System.String)",
"  [AspectMethodInsertBefore(\"System.IO.StreamWriter::.ctor(System.String,System.Boolean,System.Text.Encoding,System.Int32)\",\"\",[3],[False],[None],Propagation,[])] ReviewPath(System.String)",
"[AspectClass(\"mscorlib,netstandard,System.Runtime\",[None],Propagation,[])] Datadog.Trace.Iast.Aspects.System.Text.StringBuilderAspects",
"  [AspectCtorReplace(\"System.Text.StringBuilder::.ctor(System.String)\",\"\",[0],[False],[StringLiteral_1],Propagation,[])] Init(System.String)",
"  [AspectCtorReplace(\"System.Text.StringBuilder::.ctor(System.String,System.Int32)\",\"\",[0],[False],[StringLiteral_1],Propagation,[])] Init(System.String,System.Int32)",
"  [AspectCtorReplace(\"System.Text.StringBuilder::.ctor(System.String,System.Int32,System.Int32,System.Int32)\",\"\",[0],[False],[StringLiteral_1],Propagation,[])] Init(System.String,System.Int32,System.Int32,System.Int32)",
"  [AspectMethodReplace(\"System.Object::ToString()\",\"System.Text.StringBuilder\",[0],[False],[None],Propagation,[])] ToString(System.Text.StringBuilder)",
"  [AspectMethodReplace(\"System.Text.StringBuilder::ToString(System.Int32,System.Int32)\",\"\",[0],[False],[None],Propagation,[])] ToString(System.Text.StringBuilder,System.Int32,System.Int32)",
"  [AspectMethodReplace(\"System.Text.StringBuilder::Append(System.String)\",\"\",[0],[False],[StringLiteral_1],Propagation,[])] Append(System.Text.StringBuilder,System.String)",
"  [AspectMethodReplace(\"System.Text.StringBuilder::Append(System.Text.StringBuilder)\",\"\",[0],[False],[None],Propagation,[])] Append(System.Text.StringBuilder,System.Text.StringBuilder)",
"  [AspectMethodReplace(\"System.Text.StringBuilder::Append(System.String,System.Int32,System.Int32)\",\"\",[0],[False],[StringLiteral_1],Propagation,[])] Append(System.Text.StringBuilder,System.String,System.Int32,System.Int32)",
"  [AspectMethodReplace(\"System.Text.StringBuilder::Append(System.Text.StringBuilder,System.Int32,System.Int32)\",\"\",[0],[False],[StringLiteral_1],Propagation,[])] Append(System.Text.StringBuilder,System.Text.StringBuilder,System.Int32,System.Int32)",
"  [AspectMethodReplace(\"System.Text.StringBuilder::Append(System.Char[],System.Int32,System.Int32)\",\"\",[0],[False],[None],Propagation,[])] Append(System.Text.StringBuilder,System.Char[],System.Int32,System.Int32)",
"  [AspectMethodReplace(\"System.Text.StringBuilder::Append(System.Object)\",\"\",[0],[False],[None],Propagation,[])] Append(System.Text.StringBuilder,System.Object)",
"  [AspectMethodReplace(\"System.Text.StringBuilder::Append(System.Char[])\",\"\",[0],[False],[None],Propagation,[])] Append(System.Text.StringBuilder,System.Char[])",
"  [AspectMethodReplace(\"System.Text.StringBuilder::AppendLine(System.String)\",\"\",[0],[False],[StringLiteral_1],Propagation,[])] AppendLine(System.Text.StringBuilder,System.String)",
"[AspectClass(\"mscorlib,netstandard,System.Private.CoreLib,System.Runtime\",[StringOptimization],Propagation,[])] Datadog.Trace.Iast.Aspects.System.StringAspects",
"  [AspectMethodReplace(\"System.String::Trim()\",\"\",[0],[False],[StringLiteral_0],Propagation,[])] Trim(System.String)",
"  [AspectMethodReplace(\"System.String::Trim(System.Char[])\",\"\",[0],[False],[StringLiteral_0],Propagation,[])] Trim(System.String,System.Char[])",
"  [AspectMethodReplace(\"System.String::Trim(System.Char)\",\"\",[0],[False],[StringLiteral_0],Propagation,[])] Trim(System.String,System.Char)",
"  [AspectMethodReplace(\"System.String::TrimStart(System.Char[])\",\"\",[0],[False],[StringLiteral_0],Propagation,[])] TrimStart(System.String,System.Char[])",
"  [AspectMethodReplace(\"System.String::TrimStart(System.Char)\",\"\",[0],[False],[StringLiteral_0],Propagation,[])] TrimStart(System.String,System.Char)",
"  [AspectMethodReplace(\"System.String::TrimStart()\",\"\",[0],[False],[StringLiteral_0],Propagation,[])] TrimStart(System.String)",
"  [AspectMethodReplace(\"System.String::TrimEnd(System.Char[])\",\"\",[0],[False],[StringLiteral_0],Propagation,[])] TrimEnd(System.String,System.Char[])",
"  [AspectMethodReplace(\"System.String::TrimEnd(System.Char)\",\"\",[0],[False],[StringLiteral_0],Propagation,[])] TrimEnd(System.String,System.Char)",
"  [AspectMethodReplace(\"System.String::TrimEnd()\",\"\",[0],[False],[StringLiteral_0],Propagation,[])] TrimEnd(System.String)",
"  [AspectMethodReplace(\"System.String::Concat(System.String,System.String)\",\"\",[0],[False],[StringLiterals_Any],Propagation,[])] Concat(System.String,System.String)",
"  [AspectMethodReplace(\"System.String::Concat(System.String,System.String)\",\"\",[0],[False],[StringLiteral_0],Propagation,[])] Concat_0(System.String,System.String)",
"  [AspectMethodReplace(\"System.String::Concat(System.String,System.String)\",\"\",[0],[False],[StringLiteral_1],Propagation,[])] Concat_1(System.String,System.String)",
"  [AspectMethodReplace(\"System.String::Concat(System.Object,System.Object)\",\"\",[0],[False],[None],Propagation,[])] Concat(System.Object,System.Object)",
"  [AspectMethodReplace(\"System.String::Concat(System.String,System.String,System.String)\",\"\",[0],[False],[StringLiterals],Propagation,[])] Concat(System.String,System.String,System.String)",
"  [AspectMethodReplace(\"System.String::Concat(System.Object,System.Object,System.Object)\",\"\",[0],[False],[None],Propagation,[])] Concat(System.Object,System.Object,System.Object)",
"  [AspectMethodReplace(\"System.String::Concat(System.String,System.String,System.String,System.String)\",\"\",[0],[False],[StringLiterals],Propagation,[])] Concat(System.String,System.String,System.String,System.String)",
"  [AspectMethodReplace(\"System.String::Concat(System.String[])\",\"\",[0],[False],[None],Propagation,[])] Concat(System.String[])",
"  [AspectMethodReplace(\"System.String::Concat(System.Object[])\",\"\",[0],[False],[None],Propagation,[])] Concat(System.Object[])",
"  [AspectMethodReplace(\"System.String::Concat(System.Collections.Generic.IEnumerable`1<System.String>)\",\"\",[0],[False],[None],Propagation,[])] Concat(System.Collections.IEnumerable)",
"  [AspectMethodReplace(\"System.String::Concat(System.Collections.Generic.IEnumerable`1<!!0>)\",\"\",[0],[False],[None],Propagation,[])] Concat2(System.Collections.IEnumerable)",
"  [AspectMethodReplace(\"System.String::Substring(System.Int32)\",\"\",[0],[False],[StringLiteral_0],Propagation,[])] Substring(System.String,System.Int32)",
"  [AspectMethodReplace(\"System.String::Substring(System.Int32,System.Int32)\",\"\",[0],[False],[StringLiteral_0],Propagation,[])] Substring(System.String,System.Int32,System.Int32)",
"  [AspectMethodReplace(\"System.String::ToCharArray()\",\"\",[0],[False],[StringLiteral_0],Propagation,[])] ToCharArray(System.String)",
"  [AspectMethodReplace(\"System.String::ToCharArray(System.Int32,System.Int32)\",\"\",[0],[False],[StringLiteral_0],Propagation,[])] ToCharArray(System.String,System.Int32,System.Int32)",
"  [AspectMethodReplace(\"System.String::Join(System.String,System.String[],System.Int32,System.Int32)\",\"\",[0],[False],[None],Propagation,[])] Join(System.String,System.String[],System.Int32,System.Int32)",
"  [AspectMethodReplace(\"System.String::Join(System.Char,System.String[])\",\"\",[0],[False],[None],Propagation,[])] Join(System.Char,System.String[])",
"  [AspectMethodReplace(\"System.String::Join(System.Char,System.Object[])\",\"\",[0],[False],[None],Propagation,[])] Join(System.Char,System.Object[])",
"  [AspectMethodReplace(\"System.String::Join(System.Char,System.String[],System.Int32,System.Int32)\",\"\",[0],[False],[None],Propagation,[])] Join(System.Char,System.String[],System.Int32,System.Int32)",
"  [AspectMethodReplace(\"System.String::Join(System.Char,System.Collections.Generic.IEnumerable`1<!!0>)\",\"\",[0],[False],[None],Propagation,[])] Join(System.Char,System.Collections.IEnumerable)",
"  [AspectMethodReplace(\"System.String::Join(System.String,System.Object[])\",\"\",[0],[False],[None],Propagation,[])] Join(System.String,System.Object[])",
"  [AspectMethodReplace(\"System.String::Join(System.String,System.String[])\",\"\",[0],[False],[None],Propagation,[])] Join(System.String,System.String[])",
"  [AspectMethodReplace(\"System.String::Join(System.String,System.Collections.Generic.IEnumerable`1<System.String>)\",\"\",[0],[False],[None],Propagation,[])] Join(System.String,System.Collections.IEnumerable)",
"  [AspectMethodReplace(\"System.String::Join(System.String,System.Collections.Generic.IEnumerable`1<!!0>)\",\"\",[0],[False],[None],Propagation,[])] Join2(System.String,System.Collections.IEnumerable)",
"  [AspectMethodReplace(\"System.String::ToUpper()\",\"\",[0],[False],[StringLiteral_0],Propagation,[])] ToUpper(System.String)",
"  [AspectMethodReplace(\"System.String::ToUpper(System.Globalization.CultureInfo)\",\"\",[0],[False],[StringLiteral_0],Propagation,[])] ToUpper(System.String,System.Globalization.CultureInfo)",
"  [AspectMethodReplace(\"System.String::ToUpperInvariant()\",\"\",[0],[False],[StringLiteral_0],Propagation,[])] ToUpperInvariant(System.String)",
"  [AspectMethodReplace(\"System.String::ToLower()\",\"\",[0],[False],[StringLiteral_0],Propagation,[])] ToLower(System.String)",
"  [AspectMethodReplace(\"System.String::ToLower(System.Globalization.CultureInfo)\",\"\",[0],[False],[StringLiteral_0],Propagation,[])] ToLower(System.String,System.Globalization.CultureInfo)",
"  [AspectMethodReplace(\"System.String::ToLowerInvariant()\",\"\",[0],[False],[StringLiteral_0],Propagation,[])] ToLowerInvariant(System.String)",
"  [AspectMethodReplace(\"System.String::Remove(System.Int32)\",\"\",[0],[False],[StringLiteral_0],Propagation,[])] Remove(System.String,System.Int32)",
"  [AspectMethodReplace(\"System.String::Remove(System.Int32,System.Int32)\",\"\",[0],[False],[StringLiteral_0],Propagation,[])] Remove(System.String,System.Int32,System.Int32)",
"  [AspectMethodReplace(\"System.String::Insert(System.Int32,System.String)\",\"\",[0],[False],[StringOptimization],Propagation,[])] Insert(System.String,System.Int32,System.String)",
"  [AspectMethodReplace(\"System.String::PadLeft(System.Int32)\",\"\",[0],[False],[StringLiteral_0],Propagation,[])] PadLeft(System.String,System.Int32)",
"  [AspectMethodReplace(\"System.String::PadLeft(System.Int32,System.Char)\",\"\",[0],[False],[StringLiteral_0],Propagation,[])] PadLeft(System.String,System.Int32,System.Char)",
"  [AspectMethodReplace(\"System.String::PadRight(System.Int32)\",\"\",[0],[False],[StringLiteral_0],Propagation,[])] PadRight(System.String,System.Int32)",
"  [AspectMethodReplace(\"System.String::PadRight(System.Int32,System.Char)\",\"\",[0],[False],[StringLiteral_0],Propagation,[])] PadRight(System.String,System.Int32,System.Char)",
        };
    }
}
