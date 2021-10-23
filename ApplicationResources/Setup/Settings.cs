﻿using System;
using System.Collections;
using System.Collections.Generic;
using ApplicationResources.Logging;
using CustomResources.Utils.Concepts.DataStructures;
using CustomResources.Utils.Extensions;

namespace ApplicationResources.Setup
{
	public static class Settings
	{
		public static ICollection<Enum> AllSettings => _settingsStore.AllSettings;
		public static EnumNamesDictionary AllSettingsName => _settingsStore.AllSettingsNames;
		private readonly static SettingsStore _settingsStore = new();


		public static T Get<T>(Enum setting) => TryGet<T>(setting, out var value) ? value : default;
		public static bool TryGet<T>(Enum setting, out T value) => _settingsStore.TryGetValue(setting, out value);

		public static void RegisterSettings<EnumT>() where EnumT : struct, Enum => _settingsStore.RegisterSettings(typeof(EnumT));
		public static void RegisterProviders(params ISettingsProvider[] providers) => RegisterProviders(providers);
		public static void RegisterProviders(IEnumerable<ISettingsProvider> providers) => providers.EachIndependently(RegisterProvider);
		public static void RegisterProvider(ISettingsProvider provider) => _settingsStore.RegisterProvider(provider);
		public static void Load() => _settingsStore.Load();

		public static IEnumerable<(Enum setting, string stringValue)> GetAllSettingsAsStrings() => _settingsStore.GetAllSettingsAsStrings();
	}

	public class EnumNamesDictionary : BijectiveDictionary<Enum, string>, ICollection<Enum>, IEnumerable<string>, IReadOnlyDictionary<Enum, string>, IReadOnlyDictionary<string, Enum>
	{
		public EnumNamesDictionary(bool ignoreCase = true)
			: base(destinationEquality: ignoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal)
		{ }

		public int Count => _mapping.Count;
		public bool IsReadOnly => false;

		public IEnumerable<Enum> Keys => InputSpace;
		public IEnumerable<string> Values => OutputSpace;
		IEnumerable<string> IReadOnlyDictionary<string, Enum>.Keys => OutputSpace;
		IEnumerable<Enum> IReadOnlyDictionary<string, Enum>.Values => InputSpace;

		public string this[Enum key] => _mapping[key];
		public Enum this[string key] => _inverseMapping[key];

		public void Add(Enum item) => Expand(item, item.ToString());
		public bool Contains(Enum item) => InputSpace.Contains(item);
		public void CopyTo(Enum[] array, int arrayIndex) => InputSpace.CopyTo(array, arrayIndex);
		IEnumerator<Enum> IEnumerable<Enum>.GetEnumerator() => InputSpace.GetEnumerator();
		IEnumerator<string> IEnumerable<string>.GetEnumerator() => OutputSpace.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public bool ContainsKey(Enum key) => _mapping.ContainsKey(key);
		public bool TryGetValue(Enum key, out string value) => _mapping.TryGetValue(key, out value);
		public IEnumerator<KeyValuePair<Enum, string>> GetEnumerator() => _mapping.GetEnumerator();

		public bool ContainsKey(string key) => _inverseMapping.ContainsKey(key);
		public bool TryGetValue(string key, out Enum value) => _inverseMapping.TryGetValue(key, out value);
		IEnumerator<KeyValuePair<string, Enum>> IEnumerable<KeyValuePair<string, Enum>>.GetEnumerator() => _inverseMapping.GetEnumerator();

		public bool Remove(Enum item) => throw new NotSupportedException();
		public void Clear() => throw new NotSupportedException();
	}
}
