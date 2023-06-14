using LoGiC.NET.v2.Utils;

namespace LoGiC.NET.v2.Obfuscation;

public sealed class RenameObfuscation : BaseObfuscation
{
    public override string Name => "Rename";

    private uint _renamedTypes, _renamedFields, _renamedProperties, _renamedEvents, _renamedMethods, _renamedParameters;

    public override void Run(ObfuscationContext context)
    {
        context.Module.Mvid = Guid.NewGuid();
        context.Module.EncId = Guid.NewGuid();
        context.Module.EncBaseId = Guid.NewGuid();

        context.Module.Name = NumberUtils.Random.Next().GetHashCode().ToString();

        foreach (var type in context.Module.Types)
        {
            if (type is
                {
                    IsSpecialName: false, IsRuntimeSpecialName: false, IsWindowsRuntime: false, IsPublic: false
                })
            {
                type.Namespace = NumberUtils.Random.Next().GetHashCode().ToString();
                type.Name = NumberUtils.Random.Next().GetHashCode().ToString();

                _renamedTypes++;
            }
            else
            {
                Terminal.Warn($"Found special type: {type.FullName}");
            }

            foreach (var field in type.Fields)
            {
                if (field is
                    {
                        IsSpecialName: false, IsRuntimeSpecialName: false, IsPublic: false
                    })
                {
                    field.Name = NumberUtils.Random.Next().GetHashCode().ToString();

                    _renamedFields++;
                }
                else
                {
                    Terminal.Warn($"Found special field: {field.FullName}");
                }
            }

            foreach (var property in type.Properties)
            {
                if (property is
                    {
                        IsSpecialName: false, IsRuntimeSpecialName: false
                    })
                {
                    property.Name = NumberUtils.Random.Next().GetHashCode().ToString();

                    _renamedProperties++;
                }
                else
                {
                    Terminal.Warn($"Found special property: {property.FullName}");
                }
            }

            foreach (var ev in type.Events)
            {
                if (ev is
                    {
                        IsSpecialName: false, IsRuntimeSpecialName: false
                    })
                {
                    ev.Name = NumberUtils.Random.Next().GetHashCode().ToString();

                    _renamedEvents++;
                }
                else
                {
                    Terminal.Warn($"Found special event: {ev.FullName}");
                }
            }

            foreach (var method in type.Methods)
            {
                if (method is
                    {
                        IsSpecialName: false, IsRuntimeSpecialName: false, IsRuntime: false, IsConstructor: false, IsPublic: false
                    })
                {
                    method.Name = NumberUtils.Random.Next().GetHashCode().ToString();

                    _renamedMethods++;
                }
                else
                {
                    Terminal.Warn($"Found special method: {method.FullName}");
                }

                foreach (var parameter in method.Parameters)
                {
                    if (!parameter.IsHiddenThisParameter)
                    {
                        parameter.Name = NumberUtils.Random.Next().GetHashCode().ToString();

                        _renamedParameters++;
                    }
                    else
                    {
                        Terminal.Warn($"Found special this parameter in method: {method.FullName}");
                    }
                }
            }
        }
        
        Terminal.Info($"Renamed {_renamedTypes} types, {_renamedFields} fields, {_renamedProperties} properties, {_renamedEvents} events, {_renamedMethods} methods and {_renamedParameters} parameters");
    }
}