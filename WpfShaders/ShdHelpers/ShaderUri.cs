namespace WpfRenderT
{
    using System;
    using System.Reflection;

    //Class for creating "pack://" URIs.
    //Usefull when 
    public static class ShaderUri
    {
        private static string assemblyShortName;

        private static string AssemblyShortName
        {
            get
            {
                if (assemblyShortName == null)
                {
                    Assembly a = typeof(ShaderUri).Assembly;

                    assemblyShortName = a.ToString().Split(',')[0];
                }

                return assemblyShortName;
            }
        }

        public static Uri MakePackUri(string relativeFile)
        {
            string uriString = "pack://application:,,,/" + AssemblyShortName + ";component/" + relativeFile;
            return new Uri(uriString);
        }
    }
}