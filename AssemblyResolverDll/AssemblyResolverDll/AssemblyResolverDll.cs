﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyResolverDll {
    public class AsseblyResolver {
        public void Attach() {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        public string FindDllFolder(string dllFolderName) {

            var currentPath = Directory.GetCurrentDirectory();
            while(currentPath != null) {
                var candidateName = "";
                candidateName = Path.Combine(currentPath, dllFolderName);
                if(Directory.Exists(candidateName)) {
                    return candidateName;
                } else {
                    var di = Directory.GetParent(currentPath);
                    if(di != null)
                        currentPath = di.FullName;
                    else
                        currentPath = null;
                }
            }
            return null;
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args) {
            // Ignore missing resources
            if(args.Name.Contains(".resources"))
                return null;

            // check for assemblies already loaded
            Assembly assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName == args.Name);
            if(assembly != null)
                return assembly;

            var dllFolderName = FindDllFolder("Dll1234");
            if(dllFolderName == null)
                return null;

            // Try to load by filename - split out the filename of the full assembly name
            // and append the base path of the original assembly (ie. look in the same dir)
            string filename = args.Name.Split(',')[0] + ".dll".ToLower();

            string asmFile = Path.Combine(dllFolderName, filename);

            try {
                return System.Reflection.Assembly.LoadFrom(asmFile);
            }
            catch(Exception ex) {
                return null;
            }
        }
    }
}
