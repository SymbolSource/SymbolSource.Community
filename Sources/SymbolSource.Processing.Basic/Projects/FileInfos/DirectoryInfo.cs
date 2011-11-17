using System;
using System.Collections.Generic;
using System.Linq;

namespace SymbolSource.Processing.Basic.Projects.FileInfos
{
    public abstract class DirectoryInfo : Info, IDirectoryInfo
    {
        private IList<IFileInfo> files;
        private IList<IDirectoryInfo> directories;

        protected DirectoryInfo(ISpecialDirectoryHandler specialDirectoryHandler, DirectoryInfo parentInfo)
            : base(parentInfo)
        {
            SpecialDirectoryHandler = specialDirectoryHandler;
        }

        protected ISpecialDirectoryHandler SpecialDirectoryHandler { get; private set; }
        protected abstract IEnumerable<IDirectoryInfo> ExecuteGetDirectories();
        protected abstract IEnumerable<IFileInfo> ExecuteGetFiles();

        public IEnumerable<IFileInfo> GetFiles()
        {
            if (files == null)
                files = ExecuteGetFiles().ToArray();

            return files;
        }

        public IEnumerable<IDirectoryInfo> GetDirectories()
        {
            if (directories == null)
                directories = ExecuteGetDirectories().ToArray();

            return directories;
        }

        public IDirectoryInfo GetDirectory(params string[] names)
        {
            if (names == null)
                throw new ArgumentNullException();

            IDirectoryInfo result = this;

            foreach (var name in names)
            {
                if (result == null)
                    throw new Exception(string.Format("Unable to find part of path '{0}' before '{1}'.", string.Join("/", names), name));

                result = result.GetDirectories()
                    .SingleOrDefault(d => d.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            }

            return result;
        }

        public IFileInfo GetFile(params string[] names)
        {
            if (names == null)
                throw new ArgumentNullException();

            if (names.Length == 0)
                throw new ArgumentOutOfRangeException();

            var path = names.Take(names.Length - 1).ToArray();
            var directory = GetDirectory(path);

            if (directory == null)
                throw new Exception(string.Format("Unable to find directory '{0}'.", string.Join("/", path)));

            return directory.GetFiles().SingleOrDefault(f => f.Name.Equals(names[names.Length - 1], StringComparison.OrdinalIgnoreCase));
        }

        public override void Dispose()
        {
            if (files != null)
                foreach (var file in files)
                    file.Dispose();
            
            if (directories != null)
                foreach (var directory in directories)
                    directory.Dispose();

            base.Dispose();
        }
    }
}