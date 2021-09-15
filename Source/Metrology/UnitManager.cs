// GE Aviation Systems LLC licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace GEAviation.Metrology
{
    public class UnitWithPrefix<TUnitType> where TUnitType : IUnit
    {
        public TUnitType Unit { get; set; }
        public IPrefix Prefix { get; set; }
    }

    /// <summary>
    /// This class is used to manage instances of Unit and Dimension classes. Methods of this class
    /// should be used to obtain instances of <see cref="IUnit"/> and <see cref="IDimension"/> 
    /// implementations instead of explicitly instantiating them.
    /// </summary>
    public static class UnitManager
    {
        private static Dictionary<Type, IUnit> mRegisteredUnits;
        private static Dictionary<Type, IDimension> mRegisteredDimensions;
        private static Dictionary<Type, IPrefix> mRegisteredPrefixes;

        private static Dictionary<string, UnitWithPrefix<IUnit>> mCachedAbbreviationLookup;

        /// <summary>
        /// When UnitManager is first used/loaded, it will automatically scan the current
        /// AppDomain for all <see cref="IUnit"/> and <see cref="IDimension"/> implementations 
        /// and instantiate and cache those instances.
        /// </summary>
        static UnitManager()
        {
            mRegisteredUnits = new Dictionary<Type, IUnit>();
            mRegisteredDimensions = new Dictionary<Type, IDimension>();
            mRegisteredPrefixes = new Dictionary<Type, IPrefix>();

            mCachedAbbreviationLookup = new Dictionary<string, UnitWithPrefix<IUnit>>();

            var lAllInstantiableTypes = AppDomain.CurrentDomain.GetAssemblies()
                                                               .SelectMany( aAssembly => aAssembly.GetTypes() )
                                                               .Where( aType => !aType.IsAbstract );

            var lSimpleUnitTypes = lAllInstantiableTypes.Where( aType => typeof( ISimpleUnit ).IsAssignableFrom( aType ) );
            var lDerivedUnitTypes = lAllInstantiableTypes.Where( aType => typeof( IDerivedUnit ).IsAssignableFrom( aType ) );

            // Filtering out unit types who act as their own dimension.
            var lDimensionTypes = lAllInstantiableTypes.Where( aType => typeof( IDimension ).IsAssignableFrom( aType ) && !typeof( IUnit ).IsAssignableFrom( aType ) );

            var lPrefixTypes = lAllInstantiableTypes.Where( aType => typeof( IPrefix ).IsAssignableFrom( aType ) );

            foreach ( var lUnitType in lSimpleUnitTypes )
            {
                register( lUnitType, mRegisteredUnits );
            }

            foreach ( var lDimensionType in lDimensionTypes )
            {
                register( lDimensionType, mRegisteredDimensions );
            }

            foreach ( var lPrefixType in lPrefixTypes )
            {
                register( lPrefixType, mRegisteredPrefixes );
            }

            // Derived units are specifically registered later as they rely on simple units, dimensions and prefixes.
            foreach ( var lUnitType in lDerivedUnitTypes )
            {
                register( lUnitType, mRegisteredUnits );
            }
        }

        /// <summary>
        /// Call this method to register a unit or dimension with the Metrology system. This should only
        /// be necessary for types that were not automatically registered when UnitManager
        /// was initially loaded because they were not in an Assembly that was in the current AppDomain
        /// when UnitManager was loaded.
        /// </summary>
        /// <typeparam name="UnitType">
        /// The Unit or Dimension type to register.
        /// </typeparam>
        private static void register<ObjectType>( Type aObjectType, IDictionary<Type, ObjectType> aRegistry ) where ObjectType : class
        {
            if ( !aRegistry.ContainsKey( aObjectType ) )
            {
                if ( typeof( ObjectType ).IsAssignableFrom( aObjectType ) )
                {
                    var lObject = Activator.CreateInstance( aObjectType ) as ObjectType;
                    if ( lObject != null )
                    {
                        aRegistry[aObjectType] = lObject;
                        cacheUnitWithPrefixAbbreviation( lObject );
                        return;
                    }
                    throw new Exception( $"Type {aObjectType?.Name} failed to register." );
                }

                throw new InvalidOperationException( $"Type {aObjectType?.Name} is not an implementation of {typeof( ObjectType ).Name}." );
            }
        }

        /// <summary>
        /// Caches the every prefix+unit combination for quicker look up later.
        /// </summary>
        /// <param name="aUnitOrPrefix">
        /// The <see cref="IUnit"/> or <see cref="IPrefix"/> object to perform caching for.
        /// </param>
        private static void cacheUnitWithPrefixAbbreviation( object aUnitOrPrefix )
        {
            if ( aUnitOrPrefix is IUnit lUnit )
            {
                foreach ( var lPrefix in mRegisteredPrefixes.Values )
                {
                    mCachedAbbreviationLookup[$"{lPrefix.Abbreviation}{lUnit.Abbreviation}"] = new UnitWithPrefix<IUnit>() { Unit = lUnit, Prefix = lPrefix };
                }
            }
            else if ( aUnitOrPrefix is IPrefix lPrefix )
            {
                foreach ( var lRegisteredUnit in mRegisteredUnits.Values )
                {
                    mCachedAbbreviationLookup[$"{lPrefix.Abbreviation}{lRegisteredUnit.Abbreviation}"] = new UnitWithPrefix<IUnit>() { Unit = lRegisteredUnit, Prefix = lPrefix };
                }
            }
        }

        /// <summary>
        /// Gets a unit and prefix combination from the cache by string abbreviation.
        /// </summary>
        /// <param name="aAbbreviation">
        /// The abbreviation that represents the unit+prefix combination.
        /// </param>
        /// <returns>
        /// The <see cref="UnitWithPrefix"/> object that represents the pair.
        /// </returns>
        internal static UnitWithPrefix<TUnitType> lookupUnitAndPrefix<TUnitType>( string aAbbreviation )
            where TUnitType : IUnit
        {
            if ( mCachedAbbreviationLookup.TryGetValue( aAbbreviation, out var lValue ) )
            {
                if ( lValue.Unit is TUnitType lSpecificType )
                {
                    return new UnitWithPrefix<TUnitType>() { Unit = lSpecificType, Prefix = lValue.Prefix };
                }
            }
            return null;
        }

        /// <summary>
        /// Obtains the specified <typeparamref name="UnitType"/> from the pre-loaded/registered
        /// Unit instances that have been cached. 
        /// </summary>
        /// <typeparam name="UnitType">
        /// The type of unit to retrieve.
        /// </typeparam>
        /// <returns>
        /// An instance of the requested unit type.
        /// </returns>
        public static UnitType getUnit<UnitType>() where UnitType : class, IUnit
        {
            return getUnit( typeof( UnitType ) ) as UnitType;
        }

        internal static IUnit getUnit( Type aUnitType )
        {
            if ( !mRegisteredUnits.ContainsKey( aUnitType ) )
            {
                register( aUnitType, mRegisteredUnits );
            }

            return mRegisteredUnits[aUnitType];
        }

        /// <summary>
        /// Obtains the specified <typeparamref name="DimensionType"/> from the pre-loaded/registered
        /// Dimension instances that have been cached. 
        /// </summary>
        /// <typeparam name="UnitType">
        /// The type of dimension to retrieve.
        /// </typeparam>
        /// <returns>
        /// An instance of the requested dimension type.
        /// </returns>
        public static DimensionType getDimension<DimensionType>() where DimensionType : class, IDimension
        {
            return getDimension( typeof( DimensionType ) ) as DimensionType;
        }

        internal static IDimension getDimension( Type aDimensionType )
        {
            if ( !mRegisteredUnits.ContainsKey( aDimensionType ) )
            {
                register( aDimensionType, mRegisteredDimensions );
            }

            return mRegisteredDimensions[aDimensionType];
        }

        /// <summary>
        /// Obtains the specified <typeparamref name="PrefixType"/> from the pre-loaded/registered
        /// Prefix instances that have been cached. 
        /// </summary>
        /// <typeparam name="UnitType">
        /// The type of prefix to retrieve.
        /// </typeparam>
        /// <returns>
        /// An instance of the requested prefix type.
        /// </returns>
        public static PrefixType getPrefix<PrefixType>() where PrefixType : class, IPrefix
        {
            return getPrefix( typeof( PrefixType ) ) as PrefixType;
        }

        internal static IPrefix getPrefix( Type aPrefixType )
        {
            if ( !mRegisteredUnits.ContainsKey( aPrefixType ) )
            {
                register( aPrefixType, mRegisteredPrefixes );
            }

            return mRegisteredPrefixes[aPrefixType];
        }

        public static IEnumerable<IUnit> getAllUnits()
        {
            return mRegisteredUnits.Select( aPair => aPair.Value );
        }

        public static IEnumerable<IDimension> getAllDimensions()
        {
            return mRegisteredDimensions.Select( aPair => aPair.Value );
        }

        public static IEnumerable<IPrefix> getAllPrefixes()
        {
            return mRegisteredPrefixes.Select( aPair => aPair.Value );
        }
    }
}
