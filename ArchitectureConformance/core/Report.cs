using System.Collections.Concurrent;
using System.Text.Json;
using ArchitectureConformance.models;
using ArchitetureConformance.dto;
using ArchitetureConformance.utils;

namespace ArchitectureConformance.core;

public class Report (List<Pkg> pkgs)
{
    public void OutputInconsistencies()
    {
        var totalInconsistencies = pkgs.Sum(pkg => pkg.inconsistencies.Count);
        var totalAbsences = pkgs.Sum(pkg => pkg.absences.Count);
        
        if(totalAbsences == 0 && totalInconsistencies == 0)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Nenhuma divergência ou ausência arquitetural encontrada");
            Console.ResetColor();
            return;
        }
        
        pkgs.ForEach(pkg =>
        {
            if(pkg.inconsistencies.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($"Divergências arquiteturais encontradas no pacote {pkg.mappedRelationship.namespaceRegex}");
                Console.ResetColor();
                pkg.inconsistencies.ToList().ForEach(inconsistency =>
                {
                    Console.WriteLine($"\t- Entidade {inconsistency.originEntity.classDeclaration.Identifier.Text} referencia {inconsistency.reason}");
                });
                Console.WriteLine();
            }
        });
        
        pkgs.ForEach(pkg =>
        {
            if(pkg.absences.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($"Ausências arquiteturais no pacote {pkg.mappedRelationship.namespaceRegex}");
                Console.ResetColor();

                pkg.absences.ToList().ForEach(inconsistency =>
                {
                    Console.WriteLine($"\t- Referência ausente a: {inconsistency}");
                });
                Console.WriteLine();
            }
        });


        Console.WriteLine($"Total de divergências: {totalInconsistencies}");
        Console.WriteLine($"Total de ausências: {totalAbsences}");
        
    }
    

}
