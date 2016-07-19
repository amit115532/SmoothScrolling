//------------------------------------------------------------------------------
// <copyright file="SmoothScrollingPackage.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.OLE.Interop;

namespace SmoothScrolling
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [Guid(SmoothScrollingPackage.PACKAGE_GUID_STRING)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [ProvideAutoLoad(UIContextGuids.NoSolution)]
    [ProvideOptionPage(typeof(OptionPageGrid),
    "Smooth Scrolling", "Options", 0, 0, true)]
    public sealed class SmoothScrollingPackage : Package
    {
        public static SmoothScrollingPackage Package { get; private set; }

        /// <summary>
        /// SmoothScrollingPackage GUID string.
        /// </summary>
        public const string PACKAGE_GUID_STRING = "2924f2d0-744f-4245-bb67-998bc5662145";

        /// <summary>
        /// Initializes a new instance of the <see cref="SmoothScrollingPackage"/> class.
        /// </summary>
        public SmoothScrollingPackage()
        {
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.
            Package = this;
        }

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        public OptionPageGrid GetOptions()
        {
            return (OptionPageGrid)GetDialogPage(typeof(OptionPageGrid));
        }

        #endregion
    }

    public class OptionPageGrid : DialogPage
    {
        private double scrollIntensity = 4;
        private double decelerationSpeed = 6;
        private double minimumScrollValue = 0.1;
        private bool interruptScrollingWhenInDifferentDirection = true;
        private int updateMs = 5;
        private bool enabled = true;

        [Category("Smooth Scrolling")]
        [DisplayName("Enabled")]
        [Description("Enable or disable this feature")]
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        [Category("Smooth Scrolling")]
        [DisplayName("Deceleration Speed")]
        [Description("How fast will the smooth scroll decelerate")]
        public double DecelerationSpeed
        {
            get { return decelerationSpeed; }
            set
            {
                decelerationSpeed = value;
                if (decelerationSpeed <= 0.01)
                    decelerationSpeed = 0.01;
            }
        }

        [Category("Smooth Scrolling")]
        [DisplayName("Scroll Intensity")]
        [Description("The intensity of the scrolling.\nBigger value means faster scrolling (not smoother)")]
        public double ScrollIntensity
        {
            get { return scrollIntensity; }
            set
            {
                scrollIntensity = value;
                if (scrollIntensity < 0.01) scrollIntensity = 0.01;
            }
        }

        [Category("Smooth Scrolling")]
        [DisplayName("Minimum Scroll Value")]
        [Description("Scrolling values below this will not be considered")]
        public double MinimumScrollValue
        {
            get { return minimumScrollValue; }
            set
            {
                minimumScrollValue = value;
                if (minimumScrollValue < 0.01) minimumScrollValue = 0.01f;
            }
        }

        [Category("Smooth Scrolling")]
        [DisplayName("Interrupt Scrolling")]
        [Description("Should we interrupt the scrolling if the user scrolling in a different direction?")]
        public bool InterruptScrollingWhenInDifferentDirection
        {
            get { return interruptScrollingWhenInDifferentDirection; }
            set { interruptScrollingWhenInDifferentDirection = value; }
        }

        [Category("Smooth Scrolling")]
        [DisplayName("Update Milliseconds")]
        [Description("How much time between engine updates (in miliseconds)\nIf you dont know what that means, do not change this value. It might reduce overall visual studio performance")]
        public int UpdateMs
        {
            get { return updateMs; }
            set
            {
                updateMs = value;
                if (updateMs < 1) updateMs = 1;
                if (updateMs > 1000) updateMs = 1000;
            }
        }
    }
}
