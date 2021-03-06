﻿using Amazon.JSII.Generator.Class;
using Amazon.JSII.JsonModel.Spec;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;
using System.Collections.Generic;

namespace Amazon.JSII.Generator.UnitTests.Class
{
    public class ClassPropertyGeneratorTests : GeneratorTestBase
    {
        const string Prefix = nameof(Generator) + "." + nameof(ClassPropertyGenerator) + ".";

        string Render(Property property)
        {
            ClassType classType = new ClassType
            (
                fullyQualifiedName: "myClassFqn",
                assembly: "myModule",
                name: "myClass",
                @namespace: "myNamespace",
                isAbstract: true,
                initializer: new Initializer(),
                properties: new[] { property }
            );

            Symbols.MapTypeToPackage("myClassFqn", "myPackage");
            Symbols.MapNamespace("myNamespace", "MyNamespace");
            Symbols.MapTypeName("myClassFqn", "MyClass", JsonModel.Spec.TypeKind.Class);

            ClassPropertyGenerator generator = new ClassPropertyGenerator(classType, property, Symbols, Namespaces);

            PropertyDeclarationSyntax methodSyntax = generator.CreateProperty();
            return methodSyntax.NormalizeWhitespace(elasticTrivia: true).ToString();
        }

        [Fact(DisplayName = Prefix + nameof(IncludesAttribute))]
        public void IncludesAttribute()
        {
            Property property = new Property
            (
                name: "myProperty",
                type: new TypeReference("myPropTypeFqn"),
                isImmutable: true,
                isAbstract: false,
                isProtected: false
            );

            Symbols.MapPropertyName("myClassFqn", "myProperty", "MyProperty");
            Symbols.MapTypeName("myPropTypeFqn", "MyPropType", JsonModel.Spec.TypeKind.Class);

            string actual = Render(property);
            string expected =
@"[JsiiProperty(name: ""myProperty"", typeJson: ""{\""fqn\"":\""myPropTypeFqn\""}"")]
public virtual MyPropType MyProperty
{
    get => GetInstanceProperty<MyPropType>();
}";
            Assert.Equal(expected, actual, ignoreLineEndingDifferences: true);
        }

        [Fact(DisplayName = Prefix + nameof(IncludesGetter))]
        public void IncludesGetter()
        {
            Property property = new Property
            (
                name: "myProperty",
                type: new TypeReference("myPropTypeFqn"),
                isImmutable: true,
                isAbstract: false,
                isProtected: false
            );

            Symbols.MapPropertyName("myClassFqn", "myProperty", "MyProperty");
            Symbols.MapTypeName("myPropTypeFqn", "MyPropType", JsonModel.Spec.TypeKind.Class);

            string actual = Render(property);
            string expected =
@"[JsiiProperty(name: ""myProperty"", typeJson: ""{\""fqn\"":\""myPropTypeFqn\""}"")]
public virtual MyPropType MyProperty
{
    get => GetInstanceProperty<MyPropType>();
}";
            Assert.Equal(expected, actual, ignoreLineEndingDifferences: true);
        }

        [Fact(DisplayName = Prefix + nameof(IncludesSetter))]
        public void IncludesSetter()
        {
            Property property = new Property
            (
                name: "myProperty",
                type: new TypeReference("myPropTypeFqn"),
                isImmutable: false,
                isAbstract: false,
                isProtected: false
            );

            Symbols.MapPropertyName("myClassFqn", "myProperty", "MyProperty");
            Symbols.MapTypeName("myPropTypeFqn", "MyPropType", JsonModel.Spec.TypeKind.Class);

            string actual = Render(property);
            string expected =
@"[JsiiProperty(name: ""myProperty"", typeJson: ""{\""fqn\"":\""myPropTypeFqn\""}"")]
public virtual MyPropType MyProperty
{
    get => GetInstanceProperty<MyPropType>();
    set => SetInstanceProperty(value);
}";
            Assert.Equal(expected, actual, ignoreLineEndingDifferences: true);
        }

        [Fact(DisplayName = Prefix + nameof(IncludesProtectedKeyword))]
        public void IncludesProtectedKeyword()
        {
            Property property = new Property
            (
                name: "myProperty",
                type: new TypeReference("myPropTypeFqn"),
                isImmutable: true,
                isAbstract: false,
                isProtected: true
            );

            Symbols.MapPropertyName("myClassFqn", "myProperty", "MyProperty");
            Symbols.MapTypeName("myPropTypeFqn", "MyPropType", JsonModel.Spec.TypeKind.Class);

            string actual = Render(property);
            string expected =
@"[JsiiProperty(name: ""myProperty"", typeJson: ""{\""fqn\"":\""myPropTypeFqn\""}"")]
protected virtual MyPropType MyProperty
{
    get => GetInstanceProperty<MyPropType>();
}";
            Assert.Equal(expected, actual, ignoreLineEndingDifferences: true);
        }

        [Fact(DisplayName = Prefix + nameof(SupportsStaticProperties))]
        public void SupportsStaticProperties()
        {
            Property property = new Property
            (
                name: "myProperty",
                type: new TypeReference("myPropTypeFqn"),
                isImmutable: false,
                isAbstract: false,
                isProtected: false,
                isStatic: true
            );

            Symbols.MapPropertyName("myClassFqn", "myProperty", "MyProperty");
            Symbols.MapTypeName("myPropTypeFqn", "MyPropType", JsonModel.Spec.TypeKind.Class);

            string actual = Render(property);
            string expected =
@"[JsiiProperty(name: ""myProperty"", typeJson: ""{\""fqn\"":\""myPropTypeFqn\""}"")]
public static MyPropType MyProperty
{
    get => GetStaticProperty<MyPropType>(typeof(MyClass));
    set => SetStaticProperty(typeof(MyClass), value);
}";
            Assert.Equal(expected, actual, ignoreLineEndingDifferences: true);
        }

        [Fact(DisplayName = Prefix + nameof(OptimizesConstantProperties))]
        public void OptimizesConstantProperties()
        {
            Property property = new Property
            (
                name: "myProperty",
                type: new TypeReference("myPropTypeFqn"),
                isAbstract: false,
                isProtected: false,
                isConstant: true
            );

            Symbols.MapPropertyName("myClassFqn", "myProperty", "MyProperty");
            Symbols.MapTypeName("myPropTypeFqn", "MyPropType", JsonModel.Spec.TypeKind.Class);

            string actual = Render(property);
            string expected =
@"[JsiiProperty(name: ""myProperty"", typeJson: ""{\""fqn\"":\""myPropTypeFqn\""}"")]
public static MyPropType MyProperty
{
    get;
}

= GetStaticProperty<MyPropType>(typeof(MyClass));";
            Assert.Equal(expected, actual, ignoreLineEndingDifferences: true);
        }

        [Fact(DisplayName = Prefix + nameof(DoesNotIncludeDocs))]
        public void DoesNotIncludeDocs()
        {
            Property property = new Property
            (
                name: "myProperty",
                type: new TypeReference("myPropTypeFqn"),
                isImmutable: true,
                isAbstract: false,
                isProtected: false,
                docs: new Docs(custom: new Dictionary<string, string>{{"foo", "bar"}})
            );

            Symbols.MapPropertyName("myClassFqn", "myProperty", "MyProperty");
            Symbols.MapTypeName("myPropTypeFqn", "MyPropType", JsonModel.Spec.TypeKind.Class);

            string actual = Render(property);
            string expected =
@"[JsiiProperty(name: ""myProperty"", typeJson: ""{\""fqn\"":\""myPropTypeFqn\""}"")]
public virtual MyPropType MyProperty
{
    get => GetInstanceProperty<MyPropType>();
}";
            Assert.Equal(expected, actual, ignoreLineEndingDifferences: true);
        }
    }
}
