using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace Nerosoft.Euonia.Mapping;

public class AutomapperTypeAdapter : ITypeAdapter
    {
        private readonly IServiceProvider _provider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        public AutomapperTypeAdapter(IServiceProvider provider)
        {
            _provider = provider;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <returns></returns>
        public TDestination Adapt<TSource, TDestination>(TSource source)
            where TSource : class
            where TDestination : class
        {
            return GetMapper().Map<TSource, TDestination>(source);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <returns></returns>
        public TDestination Adapt<TSource, TDestination>(TSource source, TDestination destination)
            where TSource : class
            where TDestination : class
        {
            return GetMapper().Map(source, destination);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <typeparam name="TDestination"></typeparam>
        /// <returns></returns>
        public TDestination Adapt<TDestination>(object source)
            where TDestination : class
        {
            return GetMapper().Map<TDestination>(source);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destinationType"></param>
        /// <returns></returns>
        public object Adapt(object source, Type destinationType)
        {
            var sourceType = source.GetType();
            return GetMapper().Map(source, sourceType, destinationType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <typeparam name="TDestination"></typeparam>
        /// <returns></returns>
        public TDestination Adapt<TDestination>(object source, TDestination destination)
            where TDestination : class
        {
            return GetMapper().Map(source, destination);
        }

        private IMapper GetMapper() => _provider.GetRequiredService<IMapper>();
    }