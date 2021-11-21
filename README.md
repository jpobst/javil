# Javil

Javil is a .NET library for reading Java bytecode, similar to how [Mono.Cecil](https://github.com/jbevain/cecil) is used to read .NET assemblies.

**Example:**

```csharp
var container = ContainerDefinition.ReadContainer ("commons-cli-1.5.0.jar");

foreach (var type in container.Types) {
    Console.WriteLine ($"- {type.FullName}");

    foreach (var method in type.Methods.Where (m => !m.IsConstructor))
        Console.WriteLine ($"  - {method.ReturnType.Name} {method.FullName} ({string.Join (", ", method.Parameters.Select (p => $"{p.ParameterType.Name} {p.Name}"))})");
}
```

**Output:**
```
- org.apache.commons.cli.AlreadySelectedException
  - Option getOption ()
  - OptionGroup getOptionGroup ()
- org.apache.commons.cli.AmbiguousOptionException
  - String createMessage (String option, Collection matchingOptions)
  - Collection getMatchingOptions ()
- org.apache.commons.cli.BasicParser
  - String[] flatten (Options options, String[] arguments, boolean stopAtNonOption)
- org.apache.commons.cli.CommandLine
  - void addArg (String arg)
  - void addOption (Option opt)
  - List getArgList ()
...
```
