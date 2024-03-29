﻿using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using RestSourceGenerator.Generators;
using RestSourceGenerator.Utilities;
using Xunit;

namespace RestSourceGenerator.Tests.Tests;


public class DefaultsGeneratorTest : CSharpSourceGeneratorTest<DefaultsGenerator, XUnitVerifier>
{
    protected override string DefaultTestProjectName => "SummerRest";

    [Fact]
    public Task Test_Generate_Default_Serializer_And_Auth_Repos_When_Have_No_Additional_File()
    {
        const string source = """
                              namespace SummerRest.Runtime.Attributes
                              {
                                  using System;
                                  [AttributeUsage(AttributeTargets.Interface)]
                                  public class GeneratedDefaultAttribute : Attribute
                                  {
                                      public GeneratedDefaultAttribute(string propNameInAdditionalFile, Type @default)
                                      {
                                      }
                                  }
                              }
                              namespace SummerRest.Runtime.Parsers
                              {
                                  using SummerRest.Runtime.Attributes;
                                  [GeneratedDefault("DataSerializer", typeof(DefaultDataSerializer))]
                                  public partial interface IDataSerializer
                                  {
                                  }
                                  public class DefaultDataSerializer { }
                              }
                              """;
        
        const string dataSerializerExpect = """
                                            using SummerRest.Runtime.DataStructures;
                                            using System;
                                            namespace SummerRest.Runtime.Parsers
                                            {
                                                public partial interface IDataSerializer : IDefaultSupport<IDataSerializer>
                                                {
                                                    public static IDataSerializer Current { get; set; } = (IDataSerializer)Activator.CreateInstance(Type.GetType("SummerRest.Runtime.Parsers.DefaultDataSerializer, SummerRest, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"));
                                                }
                                            }
                                            """;
        var test = new DefaultsGeneratorTest
        {
            TestState =
            {
                Sources = { source },
                ExpectedDiagnostics =
                {
                    new DiagnosticResult(new DiagnosticDescriptor(nameof(RestSourceGenerator), "No file", 
                        "Generated file named does not exist", "Debug", DiagnosticSeverity.Warning, true))
                },
                GeneratedSources =
                {
                    (typeof(DefaultsGenerator), "IDataSerializer.g.cs", dataSerializerExpect.FormatCode()),
                }
            },
            CompilerDiagnostics = CompilerDiagnostics.None
        };
        return test.RunAsync();
    }

    [Fact]
    public Task Test_Generate_Data_Serializer_And_Auth_Repos()
    {

        const string source = """
                              namespace SummerRest.Runtime.Attributes
                              {
                                  using System;
                                  [AttributeUsage(AttributeTargets.Interface)]
                                  public class GeneratedDefaultAttribute : Attribute
                                  {
                                      public GeneratedDefaultAttribute(string propNameInAdditionalFile, Type @default)
                                      {
                                      }
                                  }
                              }
                              namespace SummerRest.Runtime.Authenticate.TokenRepositories
                              { 
                                  using SummerRest.Runtime.Attributes;
                                  [GeneratedDefault("SecretRepository", typeof(int))]
                                  public partial interface IAuthDataRepository
                                  {
                                  }
                              }
                              namespace SummerRest.Runtime.Parsers
                              {
                                  using SummerRest.Runtime.Attributes;
                                  [GeneratedDefault("DataSerializer", typeof(int))]
                                  public partial interface IDataSerializer
                                  {
                                  }
                              }
                              """;
        const string jsonContent = """
                                   <SummerRestConfiguration
                                    SecretRepository="TestNamespace.TestAuthDataRepository, SummerRest, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null" 
                                    DataSerializer="TestNamespace.TestDataSerializer, SummerRest, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"/>
                                   """;

        const string dataSerializerExpect = """
                                            using SummerRest.Runtime.DataStructures;
                                            using System;
                                            namespace SummerRest.Runtime.Parsers
                                            {
                                                public partial interface IDataSerializer : IDefaultSupport<IDataSerializer>
                                                {
                                                    public static IDataSerializer Current { get; set; } = (IDataSerializer)Activator.CreateInstance(Type.GetType("TestNamespace.TestDataSerializer, SummerRest, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"));
                                                }
                                            }
                                            """;
        const string authReposExpect = """
                                       using SummerRest.Runtime.DataStructures;
                                       using System;
                                       namespace SummerRest.Runtime.Authenticate.TokenRepositories
                                       {
                                           public partial interface IAuthDataRepository : IDefaultSupport<IAuthDataRepository>
                                           {
                                                public static IAuthDataRepository Current { get; set; } = (IAuthDataRepository)Activator.CreateInstance(Type.GetType("TestNamespace.TestAuthDataRepository, SummerRest, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"));
                                           }
                                       }
                                       """;
        
        var test = new DefaultsGeneratorTest
        {
            TestState =
            {
                Sources = { source },
                AdditionalFiles = { ("summer-rest-generated.RestSourceGenerator.additionalfile", jsonContent) },
                GeneratedSources =
                {
                    (typeof(DefaultsGenerator), "IAuthDataRepository.g.cs", authReposExpect.FormatCode()),
                    (typeof(DefaultsGenerator), "IDataSerializer.g.cs", dataSerializerExpect.FormatCode()),
                }
            },
            CompilerDiagnostics = CompilerDiagnostics.None
        };

        return test.RunAsync();
    } 
}