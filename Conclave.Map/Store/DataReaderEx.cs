using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Conclave.Data.Store;
using Conclave.Map.Model;

namespace Conclave.Map.Store {
	public static class DataReaderEx {
		public  static Association.Builder ReadAssociation(this IDataReader self) {
			Association.Builder builder = new Association.Builder()
			{
				Id = self.ReadString("assoc_id"),
				Parent = self.ReadString("assoc_parent"),
				Scope = self.ReadString("assoc_scope"),
				Type = self.ReadString("assoc_type"),
				Role = self.ReadString("assoc_role"),
				Reference = self.ReadString("assoc_ref")
			};
			return builder;
		}

		public static Metadata.Builder ReadMetadata(this IDataReader self) {
			Metadata.Builder builder = null;
			string parent = self.ReadString("meta_parent");
			if (!String.IsNullOrWhiteSpace(parent)) {
				builder = new Metadata.Builder()
				{
					Parent = self.ReadString("meta_parent"),
					Scope = self.ReadString("meta_scope"),
					Name = self.ReadString("meta_name"),
					Value = self.ReadString("meta_value")
				};
			}
			return builder;
		}

		public static Metadata.Builder ReadMetadata(this IDataReader self, Node.Builder node) {
			Metadata.Builder metadata = self.ReadMetadata();
			if (metadata != null) {
				node.AddMetadata(metadata);
			}
			return metadata;
		}
	}
}
