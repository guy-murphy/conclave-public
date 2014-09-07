using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Conclave.Collections;
using Newtonsoft.Json;

namespace Conclave.Map.Store.MySql {
	public class FileStoreResource: IData {
		//private class ResourceIdentityComparer: IEqualityComparer<FileStoreResource> {
		//	public bool Equals(FileStoreResource x, FileStoreResource y) {
		//		throw new NotImplementedException();
		//	}

		//	public int GetHashCode(FileStoreResource obj) {
		//		throw new NotImplementedException();
		//	}
		//}

		//private class ResourceDateComparer: IComparer<FileStoreResource> {
		//	public int Compare(FileStoreResource x, FileStoreResource y) {
		//		throw new NotImplementedException();
		//	}
		//}

		public static bool operator ==(FileStoreResource o1, FileStoreResource o2) {
			return (object)o1 != null && o1.Equals(o2);
		}

		public static bool operator !=(FileStoreResource o1, FileStoreResource o2) {
			return (object)o1 != null && !o1.Equals(o2);
		}

		private static readonly FileStoreResource _blank = new FileStoreResource.Buidler()
		{
			Id = String.Empty,
			Namespace = String.Empty,
			Name = string.Empty,
			Type = String.Empty,
			Created = default(DateTime),
			Opened = default(DateTime),
			Commited = default(DateTime),
			CreatedBy = String.Empty,
			LockedBy = String.Empty,
			Rights = 0,
			Description = String.Empty,
			StringEncoding = new UTF8Encoding()
		};

		public static FileStoreResource Blank {
			get { return _blank; }
		}

		private readonly string _id;
		private readonly string _namespace;
		private readonly string _name;
		private readonly string _type;
		private readonly DateTime _created;
		private readonly DateTime _opened;
		private readonly DateTime _commited;
		private readonly string _createdBy;
		private readonly string _lockedBy;
		private readonly int _rights;
		private readonly string _description;
		private readonly byte[] _data;
		private readonly Encoding _encoding = new UTF8Encoding();

		private int _hash = 0; // for memoizing the hashcode as this class is immutable

		public string Id {
			get { return _id; }
		}

		public string Namespace {
			get { return _namespace; }
		}

		public string Name {
			get { return _name; }
		}

		public string Type {
			get { return _type; }
		}

		public DateTime CreatedOn {
			get { return _created; }
		}

		public DateTime OpenedOn {
			get { return _opened; }
		}

		public DateTime CommitedOn {
			get { return _commited; }
		}

		public string CreatedBy {
			get { return _createdBy; }
		}

		public string LockedBy {
			get { return _lockedBy; }
		}
		
		public int Rights {
			get { return _rights; }
		}

		public string Description {
			get { return _description; }
		}

		public byte[] Data {
			get { return _data; }
		}

		public decimal Size {
			get {
				return (_data == null) ? 0 : Decimal.Round(((decimal)this.Data.Length / 1024), 2);
			}
		}

		public Encoding StringEncoding {
			get { return _encoding; }
		}

		public bool HasData {
			get { return _data != null; }
		}

		public string StringData {
			get { return (this.HasData) ? this.StringEncoding.GetString(this.Data) : String.Empty; }
		}

		public FileStoreResource(string id, string ns, string name, string type, DateTime created, DateTime opened,
		                         DateTime commited, string createdBy, string lockedBy, int rights, string description, byte[] data) {
			_id = id;
			_namespace = ns;
			_name = name;
			_type = type;
			_created = created;
			_opened = opened;
			_commited = commited;
			_createdBy = createdBy;
			_lockedBy = lockedBy;
			_rights = rights;
			_description = description;
			_data = data;
		}

		public FileStoreResource(FileStoreResource resource)
			: this(resource.Id, resource.Namespace, resource.Name, resource.Type, resource.CreatedOn, resource.OpenedOn, resource.CommitedOn, resource.CreatedBy, resource.LockedBy, resource.Rights, resource.Description, resource.Data) {
		}

		object ICloneable.Clone() {
			return new FileStoreResource(this);
		}

		public FileStoreResource Clone() {
			return new FileStoreResource(this);
		}

		public override bool Equals(object obj) {
			FileStoreResource other = obj as FileStoreResource;
			return (other != null) && this.Equals(other);
		}

		/// <summary>
		/// Determines whether or not this resource is logically
		/// equal to another.
		/// </summary>
		/// <param name="other">The other resource to compare with this one.</param>
		/// <returns>
		/// Returns <b>true</b> if the two resources are logically the same;
		/// otherwise, return <b>false</b>.
		/// </returns>
		/// <remarks>
		/// This method does <b>not</b> compare <see cref="Data"/> to determine
		/// equality.
		/// </remarks>

		public bool Equals(FileStoreResource other) {
			return Object.ReferenceEquals(this, other) || (
					this.Id == other.Id &&
					this.Namespace == other.Namespace &&
					this.Name == other.Name &&
					this.Type == other.Type &&
					this.CreatedOn == other.CreatedOn &&
					this.OpenedOn == other.OpenedOn &&
					this.CommitedOn == other.CommitedOn &&
					this.CreatedBy == other.CreatedBy &&
					this.LockedBy == other.LockedBy &&
					this.Rights == other.Rights &&
					this.Description == other.Description
				);
		}

		public override int GetHashCode() {
			if (_hash == 0) {
				_hash = 1;
				_hash = _hash*31 + "FileStoreResource::".GetHashCode();
				_hash = _hash*31 + this.Id.GetHashCode();
				_hash = _hash * 31 + this.Namespace.GetHashCode();
				_hash = _hash * 31 + this.Name.GetHashCode();
				_hash = _hash * 31 + this.Type.GetHashCode();
				_hash = _hash * 31 + this.CreatedOn.GetHashCode();
				_hash = _hash * 31 + this.OpenedOn.GetHashCode();
				_hash = _hash * 31 + this.CommitedOn.GetHashCode();
				_hash = _hash * 31 + this.CreatedBy.GetHashCode();
				_hash = _hash * 31 + this.LockedBy.GetHashCode();
				_hash = _hash * 31 + this.Rights.GetHashCode();
				_hash = _hash * 31 + this.Description.GetHashCode();
			}
			return _hash;
		}

		public FileStoreResource Mutate(Func<Buidler, FileStoreResource> mutator) {
			Buidler builder = new Buidler(this);
			return mutator(builder);
		}

		public void ToXml(XmlWriter writer) {
			writer.WriteStartElement("resource");
			writer.WriteAttributeString("id", this.Id);
			writer.WriteAttributeString("namespace", this.Namespace);
			writer.WriteAttributeString("name", this.Name);
			writer.WriteAttributeString("type", this.Namespace);
			writer.WriteAttributeString("created-on", this.Namespace);
			writer.WriteAttributeString("opened-on", this.Namespace);
			writer.WriteAttributeString("commited-on", this.Namespace);
			writer.WriteAttributeString("created-by", this.Namespace);
			writer.WriteAttributeString("locked-by", this.Namespace);
			writer.WriteAttributeString("size", this.Size.ToString("##.##"));
			writer.WriteAttributeString("has-data", this.HasData.ToString());

			writer.WriteElementString("description", this.Description);
			writer.WriteStartElement("data");
			writer.WriteCData(this.StringData); // cdata
			writer.WriteEndElement();

			writer.WriteEndElement();
		}

		public void ToJson(JsonWriter writer) {
			throw new NotImplementedException();
		}

		public class Buidler {

			public static implicit operator FileStoreResource(Buidler builder) {
				return builder.ToFileStoreResource();
			}

			public static implicit operator Buidler(FileStoreResource occurrence) {
				return new Buidler(occurrence);
			}

			public enum OperationType {
				Unknown, Create, Open, Commit, Delete
			}

			//public static implicit operator FileStoreResource(Builder builder) {
			//	return builder.ToFileStoreResource();
			//}

			//public static implicit operator Builder(FileStoreResource topic) {
			//	return new Builder(topic);
			//}

			// Think more carefully on this as this is implimentation specific.
			private static readonly DateTime _zeroTime = default(DateTime);

			public string Id { get; set; }
			public string Namespace { get; set; }
			public string Name { get; set; }
			public string Type { get; set; }
			public DateTime Created { get; set; }
			public DateTime Opened { get; set; }
			public DateTime Commited { get; set; }
			public string CreatedBy { get; set; }
			public string LockedBy { get; set; }
			public int Rights { get; set; }
			public string Description { get; set; }

			public Encoding StringEncoding { get; set; }
			public byte[] Data { get; set; }
			public string StringData {
				get { return (this.Data != null) ? this.StringEncoding.GetString(this.Data) : String.Empty; }
				set { this.Data = this.StringEncoding.GetBytes(value); }
			}

			public OperationType Operation {
				get {
					if (this.Opened == _zeroTime && this.Commited != _zeroTime) {
						return OperationType.Create;
					}
					if (this.Opened != _zeroTime && this.Commited == _zeroTime) {
						return OperationType.Open;
					}
					if (this.Opened != _zeroTime && this.Commited != _zeroTime) {
						if (this.LockedBy == "DELETED") {
							return OperationType.Delete;
						} else {
							return OperationType.Commit;
						}
					}
					return OperationType.Unknown;
				}
			}

			public Buidler() {
				this.Id = Guid.NewGuid().ToString();
				this.Namespace = "default";
				this.Name = Guid.NewGuid().ToString().Substring(0, 8); // short guid
				this.Type = "unknown";
				this.Created = DateTime.UtcNow;
				this.Opened = _zeroTime;
				this.Commited = _zeroTime;
				this.CreatedBy = String.Empty;
				this.LockedBy = String.Empty;
				this.Rights = 0;
				this.Description = String.Empty;
				this.StringEncoding = new UTF8Encoding();
			}

			public Buidler(FileStoreResource resource) {
				this.FromFileStoreResource(resource);
			}

			public Buidler FromFileStoreResource(FileStoreResource resource) {
				return null;
			}

			public FileStoreResource ToFileStoreResource() {
				return null;
			}

			public DateTime GetDateTimeForComparison() {
				DateTime result;
				switch (this.Operation) {
					case OperationType.Create:
						result = this.Commited;
						break;
					case OperationType.Open:
						result = this.Opened;
						break;
					case OperationType.Commit:
						result = this.Commited;
						break;
					case OperationType.Delete:
						result = this.Commited;
						break;
					default:
						throw new Exception("Unrecognised filestore operation.");
				}
				return result;
			}

			public int CompareTo(FileStoreResource.Buidler other) {
				return other.GetDateTimeForComparison().CompareTo(this.GetDateTimeForComparison());
			}

			public int CompareTo(object obj) {
				FileStoreResource.Buidler other = obj as FileStoreResource.Buidler;
				if (other == null) throw new ArgumentException("Object for comparison is not a FileStoreResource.Builder.");

				return this.CompareTo(other);
			}
		}

	}
}
