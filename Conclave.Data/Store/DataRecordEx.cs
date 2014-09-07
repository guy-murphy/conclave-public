using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Conclave.Data.Store {
	public static class IDataRecordExtensions {

		public static object ReadObject(this IDataRecord self, string name) {
			int ordinal = self.GetOrdinal(name);
			return self.IsDBNull(ordinal) ? null : self.GetValue(ordinal);
		}

		public static string ReadString(this IDataRecord self, string name) {
			int ordinal = self.GetOrdinal(name);
			return self.IsDBNull(ordinal) ? null : self.GetString(ordinal);
		}

		public static int ReadInt(this IDataRecord self, string name) {
			int ordinal = self.GetOrdinal(name);
			return self.IsDBNull(ordinal) ? 0 : self.GetInt32(ordinal);
		}

		public static DateTime? ReadDateTimeOrNull(IDataRecord self, string name) {
			try {
				DateTime? value = null;
				int ordinal = self.GetOrdinal(name);
				if (!self.IsDBNull(ordinal)) {
					value = self.GetDateTime(ordinal);
				}
				return value;
			} catch {
				return null;
			}
		}

		public static DateTime ReadDateTime(IDataRecord self, string name) {
			int ordinal = self.GetOrdinal(name);
			return self.GetDateTime(ordinal);
		}

		public static Byte[] ReadBinaryData(IDataRecord self, string name) {
			try {
				int ordinal = self.GetOrdinal(name);
				if (self.IsDBNull(ordinal)) {
					return null;
				} else {
					byte[] buffer = (byte[])self.GetValue(ordinal);
					return buffer;
				}
			} catch (Exception) {
				return null;
			}
		}

	}
}
