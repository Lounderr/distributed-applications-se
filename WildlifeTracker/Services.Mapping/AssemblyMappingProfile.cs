using System.Reflection;

using AutoMapper;

namespace WildlifeTracker.Services.Mapping
{
    public class AssemblyMappingProfile : Profile
    {
        public AssemblyMappingProfile()
        {
            this.ApplyMappingsFromAssembly(Assembly.GetExecutingAssembly());
        }

        private void ApplyMappingsFromAssembly(Assembly assembly)
        {
            var types = assembly.GetExportedTypes().ToList();

            // IMapFrom<>  
            foreach (var map in GetFromMaps(types))
            {
                this.CreateMap(map.Source, map.Destination);
            }

            // IMapTo<>  
            foreach (var map in GetToMaps(types))
            {
                this.CreateMap(map.Source, map.Destination);
            }
        }

        private static IEnumerable<TypesMap> GetFromMaps(IEnumerable<Type> types) =>
            types
                .SelectMany(t => t
                    .GetTypeInfo()
                    .GetInterfaces()
                    .Where(i =>
                        i.GetTypeInfo().IsGenericType &&
                        i.GetGenericTypeDefinition() == typeof(IMapFrom<>) &&
                        !t.GetTypeInfo().IsAbstract &&
                        !t.GetTypeInfo().IsInterface)
                    .Select(i => new TypesMap(
                        i.GetTypeInfo().GetGenericArguments()[0],
                        t
                    )));

        private static IEnumerable<TypesMap> GetToMaps(IEnumerable<Type> types) =>
            types
                .SelectMany(t => t
                    .GetTypeInfo()
                    .GetInterfaces()
                    .Where(i =>
                        i.GetTypeInfo().IsGenericType &&
                        i.GetGenericTypeDefinition() == typeof(IMapTo<>) &&
                        !t.GetTypeInfo().IsAbstract &&
                        !t.GetTypeInfo().IsInterface)
                    .Select(i => new TypesMap(
                        t,
                        i.GetTypeInfo().GetGenericArguments()[0]
                    )));

        private record TypesMap(Type Source, Type Destination);
    }
}
