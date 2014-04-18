﻿namespace Microsoft.VisualStudio.Composition.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Composition;
    using System.Composition.Hosting;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;
    using MefV1 = System.ComponentModel.Composition;

    [Trait("GenericExports", "Open")]
    public class OpenGenericExportTests
    {
        [MefFact(CompositionEngines.V1Compat | CompositionEngines.V2Compat, typeof(Useful<>), typeof(User))]
        public void AcquireOpenGenericExport(IContainer container)
        {
            Useful<int> useful = container.GetExportedValue<Useful<int>>();
            Assert.NotNull(useful);
        }

        [MefFact(CompositionEngines.V1Compat | CompositionEngines.V2Compat, typeof(Useful<>), typeof(User))]
        public void AcquireExportWithImportOfOpenGenericExport(IContainer container)
        {
            User user = container.GetExportedValue<User>();
            Assert.NotNull(user);
            Assert.NotNull(user.Useful);
        }

        [MefFact(CompositionEngines.V1Compat | CompositionEngines.V2Compat, typeof(Useful<>), typeof(ImportManyUser))]
        public void AcquireExportWithImportManyOfOpenGenericExport(IContainer container)
        {
            var user = container.GetExportedValue<ImportManyUser>();
            Assert.NotNull(user);
            Assert.NotNull(user.Useful);
            Assert.Equal(1, user.Useful.Length);
            Assert.IsType<Useful<int>>(user.Useful[0]);
        }

        [MefFact(CompositionEngines.V1Compat | CompositionEngines.V2Compat, typeof(Useful<>), typeof(ImportManyLazyUser))]
        public void AcquireExportWithImportManyLazyOfOpenGenericExport(IContainer container)
        {
            var user = container.GetExportedValue<ImportManyLazyUser>();
            Assert.NotNull(user);
            Assert.NotNull(user.Useful);
            Assert.Equal(1, user.Useful.Length);
            Assert.IsType<Useful<int>>(user.Useful[0].Value);
        }

        [MefFact(CompositionEngines.V3EmulatingV1 | CompositionEngines.V3EmulatingV2, typeof(Useful<>), typeof(ImportManyILazyUser))]
        public void AcquireExportWithImportManyILazyOfOpenGenericExport(IContainer container)
        {
            var user = container.GetExportedValue<ImportManyILazyUser>();
            Assert.NotNull(user);
            Assert.NotNull(user.Useful);
            Assert.Equal(1, user.Useful.Length);
            Assert.IsType<Useful<int>>(user.Useful[0].Value);
        }

        [MefFact(CompositionEngines.V1 | CompositionEngines.V2, typeof(Useful<>))]
        [Trait("Container.GetExport", "Plural")]
        public void GetExportedValuesOfOpenGenericExport(IContainer container)
        {
            var usefuls = container.GetExportedValues<Useful<int>>();
            Assert.Equal(1, usefuls.Count());
            Assert.IsType<Useful<int>>(usefuls.First());
        }

        [MefFact(CompositionEngines.V1 | CompositionEngines.V2, typeof(Useful<>))]
        [Trait("Container.GetExport", "Plural")]
        public void GetExportsOfLazyOpenGenericExport(IContainer container)
        {
            var usefuls = container.GetExports<Useful<int>>();
            Assert.Equal(1, usefuls.Count());
            Assert.IsType<Useful<int>>(usefuls.First().Value);
        }

        [MefFact(CompositionEngines.V1Compat | CompositionEngines.V2Compat, typeof(OpenGenericPartWithExportingProperty<>), InvalidConfiguration = true)]
        public void ExportingPropertyOnGenericPart(IContainer container)
        {
            string result = container.GetExportedValue<string>();
        }

        [MefFact(CompositionEngines.V1Compat, typeof(OpenGenericPartWithPrivateExportingField<>), InvalidConfiguration = true)]
        public void ExportingFieldOnGenericPart(IContainer container)
        {
            string result = container.GetExportedValue<string>();
        }

        [Export]
        [MefV1.Export, MefV1.PartCreationPolicy(MefV1.CreationPolicy.NonShared)]
        public class Useful<T> { }

        [Export]
        [MefV1.Export, MefV1.PartCreationPolicy(MefV1.CreationPolicy.NonShared)]
        public class User
        {
            [Import]
            [MefV1.Import]
            public Useful<int> Useful { get; set; }
        }

        [Export]
        [MefV1.Export, MefV1.PartCreationPolicy(MefV1.CreationPolicy.NonShared)]
        public class ImportManyUser
        {
            [ImportMany]
            [MefV1.ImportMany]
            public Useful<int>[] Useful { get; set; }
        }

        [Export]
        [MefV1.Export, MefV1.PartCreationPolicy(MefV1.CreationPolicy.NonShared)]
        public class ImportManyLazyUser
        {
            [ImportMany]
            [MefV1.ImportMany]
            public Lazy<Useful<int>>[] Useful { get; set; }
        }

        [Export]
        [MefV1.Export, MefV1.PartCreationPolicy(MefV1.CreationPolicy.NonShared)]
        public class ImportManyILazyUser
        {
            [ImportMany]
            [MefV1.ImportMany]
            public ILazy<Useful<int>>[] Useful { get; set; }
        }

        public class OpenGenericPartWithExportingProperty<T>
        {
            [MefV1.Export]
            [Export]
            public string ExportingProperty
            {
                get { return "Success"; }
            }
        }

        public class OpenGenericPartWithPrivateExportingField<T>
        {
            [MefV1.Export]
            public string ExportingField = "Success";
        }

        #region Non-public tests

        [Trait("Access", "NonPublic")]
        [MefFact(CompositionEngines.V1Compat | CompositionEngines.V3EmulatingV2WithNonPublic, typeof(Useful<>), typeof(UserOfNonPublicNestedType), typeof(UserOfNonPublicNestedType.NonPublicNestedType))]
        public void NonPublicTypeArgOfOpenGenericExport(IContainer container)
        {
            var user = container.GetExportedValue<UserOfNonPublicNestedType>();
            Assert.NotNull(user.Importer);
        }

        [Trait("Access", "NonPublic")]
        [MefFact(CompositionEngines.V1Compat | CompositionEngines.V3EmulatingV2WithNonPublic, typeof(InternalUseful<>), typeof(UserOfNonPublicNestedType), typeof(UserOfNonPublicNestedType.NonPublicNestedType))]
        public void NonPublicTypeArgOfOpenGenericExportWithNonPublicPart(IContainer container)
        {
            var user = container.GetExportedValue<UserOfNonPublicNestedType>();
            Assert.NotNull(user.Importer);
        }

        [Export]
        [MefV1.Export, MefV1.PartCreationPolicy(MefV1.CreationPolicy.NonShared)]
        public class UserOfNonPublicNestedType
        {
            [Import, MefV1.Import]
            internal Useful<NonPublicNestedType> Importer { get; set; }

            internal class NonPublicNestedType { }
        }


        [Export(typeof(Useful<>))]
        [MefV1.Export(typeof(Useful<>)), MefV1.PartCreationPolicy(MefV1.CreationPolicy.NonShared)]
        internal class InternalUseful<T> : Useful<T> { }

        #endregion
    }
}
