﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Austin.DkpLib
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="DKP")]
	public partial class DkpDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    partial void InsertBillSplit(BillSplit instance);
    partial void UpdateBillSplit(BillSplit instance);
    partial void DeleteBillSplit(BillSplit instance);
    partial void InsertTransaction(Transaction instance);
    partial void UpdateTransaction(Transaction instance);
    partial void DeleteTransaction(Transaction instance);
    partial void InsertPerson(Person instance);
    partial void UpdatePerson(Person instance);
    partial void DeletePerson(Person instance);
    partial void InsertPaymentIdentity(PaymentIdentity instance);
    partial void UpdatePaymentIdentity(PaymentIdentity instance);
    partial void DeletePaymentIdentity(PaymentIdentity instance);
    partial void InsertPaymentMethod(PaymentMethod instance);
    partial void UpdatePaymentMethod(PaymentMethod instance);
    partial void DeletePaymentMethod(PaymentMethod instance);
    #endregion
		
		public DkpDataContext() : 
				base(global::Austin.DkpLib.Properties.Settings.Default.DKPConnectionString, mappingSource)
		{
			OnCreated();
		}
		
		public DkpDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public DkpDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public DkpDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public DkpDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<BillSplit> BillSplits
		{
			get
			{
				return this.GetTable<BillSplit>();
			}
		}
		
		public System.Data.Linq.Table<Transaction> Transactions
		{
			get
			{
				return this.GetTable<Transaction>();
			}
		}
		
		public System.Data.Linq.Table<Person> People
		{
			get
			{
				return this.GetTable<Person>();
			}
		}
		
		public System.Data.Linq.Table<PaymentIdentity> PaymentIdentities
		{
			get
			{
				return this.GetTable<PaymentIdentity>();
			}
		}
		
		public System.Data.Linq.Table<PaymentMethod> PaymentMethods
		{
			get
			{
				return this.GetTable<PaymentMethod>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.BillSplit")]
	public partial class BillSplit : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _ID;
		
		private string _Name;
		
		private EntitySet<Transaction> _Transactions;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIDChanging(int value);
    partial void OnIDChanged();
    partial void OnNameChanging(string value);
    partial void OnNameChanged();
    #endregion
		
		public BillSplit()
		{
			this._Transactions = new EntitySet<Transaction>(new Action<Transaction>(this.attach_Transactions), new Action<Transaction>(this.detach_Transactions));
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ID", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int ID
		{
			get
			{
				return this._ID;
			}
			set
			{
				if ((this._ID != value))
				{
					this.OnIDChanging(value);
					this.SendPropertyChanging();
					this._ID = value;
					this.SendPropertyChanged("ID");
					this.OnIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Name", DbType="NVarChar(50) NOT NULL", CanBeNull=false)]
		public string Name
		{
			get
			{
				return this._Name;
			}
			set
			{
				if ((this._Name != value))
				{
					this.OnNameChanging(value);
					this.SendPropertyChanging();
					this._Name = value;
					this.SendPropertyChanged("Name");
					this.OnNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="BillSplit_Transaction", Storage="_Transactions", ThisKey="ID", OtherKey="BillID")]
		public EntitySet<Transaction> Transactions
		{
			get
			{
				return this._Transactions;
			}
			set
			{
				this._Transactions.Assign(value);
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		private void attach_Transactions(Transaction entity)
		{
			this.SendPropertyChanging();
			entity.BillSplit = this;
		}
		
		private void detach_Transactions(Transaction entity)
		{
			this.SendPropertyChanging();
			entity.BillSplit = null;
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.[Transaction]")]
	public partial class Transaction : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private System.Guid _ID;
		
		private int _DebtorID;
		
		private int _CreditorID;
		
		private int _Amount;
		
		private System.DateTime _Created;
		
		private string _Description;
		
		private System.Nullable<int> _BillID;
		
		private EntityRef<BillSplit> _BillSplit;
		
		private EntityRef<Person> _Creditor;
		
		private EntityRef<Person> _Debtor;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIDChanging(System.Guid value);
    partial void OnIDChanged();
    partial void OnDebtorIDChanging(int value);
    partial void OnDebtorIDChanged();
    partial void OnCreditorIDChanging(int value);
    partial void OnCreditorIDChanged();
    partial void OnAmountChanging(int value);
    partial void OnAmountChanged();
    partial void OnCreatedChanging(System.DateTime value);
    partial void OnCreatedChanged();
    partial void OnDescriptionChanging(string value);
    partial void OnDescriptionChanged();
    partial void OnBillIDChanging(System.Nullable<int> value);
    partial void OnBillIDChanged();
    #endregion
		
		public Transaction()
		{
			this._BillSplit = default(EntityRef<BillSplit>);
			this._Creditor = default(EntityRef<Person>);
			this._Debtor = default(EntityRef<Person>);
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ID", DbType="UniqueIdentifier NOT NULL", IsPrimaryKey=true)]
		public System.Guid ID
		{
			get
			{
				return this._ID;
			}
			set
			{
				if ((this._ID != value))
				{
					this.OnIDChanging(value);
					this.SendPropertyChanging();
					this._ID = value;
					this.SendPropertyChanged("ID");
					this.OnIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_DebtorID", DbType="Int NOT NULL")]
		public int DebtorID
		{
			get
			{
				return this._DebtorID;
			}
			set
			{
				if ((this._DebtorID != value))
				{
					if (this._Debtor.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnDebtorIDChanging(value);
					this.SendPropertyChanging();
					this._DebtorID = value;
					this.SendPropertyChanged("DebtorID");
					this.OnDebtorIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_CreditorID", DbType="Int NOT NULL")]
		public int CreditorID
		{
			get
			{
				return this._CreditorID;
			}
			set
			{
				if ((this._CreditorID != value))
				{
					if (this._Creditor.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnCreditorIDChanging(value);
					this.SendPropertyChanging();
					this._CreditorID = value;
					this.SendPropertyChanged("CreditorID");
					this.OnCreditorIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Amount", DbType="Int NOT NULL")]
		public int Amount
		{
			get
			{
				return this._Amount;
			}
			set
			{
				if ((this._Amount != value))
				{
					this.OnAmountChanging(value);
					this.SendPropertyChanging();
					this._Amount = value;
					this.SendPropertyChanged("Amount");
					this.OnAmountChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Created", DbType="DateTime2 NOT NULL")]
		public System.DateTime Created
		{
			get
			{
				return this._Created;
			}
			set
			{
				if ((this._Created != value))
				{
					this.OnCreatedChanging(value);
					this.SendPropertyChanging();
					this._Created = value;
					this.SendPropertyChanged("Created");
					this.OnCreatedChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Description", DbType="NVarChar(50) NOT NULL", CanBeNull=false)]
		public string Description
		{
			get
			{
				return this._Description;
			}
			set
			{
				if ((this._Description != value))
				{
					this.OnDescriptionChanging(value);
					this.SendPropertyChanging();
					this._Description = value;
					this.SendPropertyChanged("Description");
					this.OnDescriptionChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_BillID", DbType="Int")]
		public System.Nullable<int> BillID
		{
			get
			{
				return this._BillID;
			}
			set
			{
				if ((this._BillID != value))
				{
					if (this._BillSplit.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnBillIDChanging(value);
					this.SendPropertyChanging();
					this._BillID = value;
					this.SendPropertyChanged("BillID");
					this.OnBillIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="BillSplit_Transaction", Storage="_BillSplit", ThisKey="BillID", OtherKey="ID", IsForeignKey=true)]
		public BillSplit BillSplit
		{
			get
			{
				return this._BillSplit.Entity;
			}
			set
			{
				BillSplit previousValue = this._BillSplit.Entity;
				if (((previousValue != value) 
							|| (this._BillSplit.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._BillSplit.Entity = null;
						previousValue.Transactions.Remove(this);
					}
					this._BillSplit.Entity = value;
					if ((value != null))
					{
						value.Transactions.Add(this);
						this._BillID = value.ID;
					}
					else
					{
						this._BillID = default(Nullable<int>);
					}
					this.SendPropertyChanged("BillSplit");
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="Person_Transaction", Storage="_Creditor", ThisKey="CreditorID", OtherKey="ID", IsForeignKey=true)]
		public Person Creditor
		{
			get
			{
				return this._Creditor.Entity;
			}
			set
			{
				Person previousValue = this._Creditor.Entity;
				if (((previousValue != value) 
							|| (this._Creditor.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._Creditor.Entity = null;
						previousValue.Credits.Remove(this);
					}
					this._Creditor.Entity = value;
					if ((value != null))
					{
						value.Credits.Add(this);
						this._CreditorID = value.ID;
					}
					else
					{
						this._CreditorID = default(int);
					}
					this.SendPropertyChanged("Creditor");
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="Person_Transaction1", Storage="_Debtor", ThisKey="DebtorID", OtherKey="ID", IsForeignKey=true)]
		public Person Debtor
		{
			get
			{
				return this._Debtor.Entity;
			}
			set
			{
				Person previousValue = this._Debtor.Entity;
				if (((previousValue != value) 
							|| (this._Debtor.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._Debtor.Entity = null;
						previousValue.Debts.Remove(this);
					}
					this._Debtor.Entity = value;
					if ((value != null))
					{
						value.Debts.Add(this);
						this._DebtorID = value.ID;
					}
					else
					{
						this._DebtorID = default(int);
					}
					this.SendPropertyChanged("Debtor");
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.Person")]
	public partial class Person : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _ID;
		
		private string _FirstName;
		
		private string _LastName;
		
		private bool _IsDeleted;
		
		private string _Email;
		
		private EntitySet<Transaction> _Credits;
		
		private EntitySet<Transaction> _Debts;
		
		private EntitySet<PaymentIdentity> _PaymentIdentities;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIDChanging(int value);
    partial void OnIDChanged();
    partial void OnFirstNameChanging(string value);
    partial void OnFirstNameChanged();
    partial void OnLastNameChanging(string value);
    partial void OnLastNameChanged();
    partial void OnIsDeletedChanging(bool value);
    partial void OnIsDeletedChanged();
    partial void OnEmailChanging(string value);
    partial void OnEmailChanged();
    #endregion
		
		public Person()
		{
			this._Credits = new EntitySet<Transaction>(new Action<Transaction>(this.attach_Credits), new Action<Transaction>(this.detach_Credits));
			this._Debts = new EntitySet<Transaction>(new Action<Transaction>(this.attach_Debts), new Action<Transaction>(this.detach_Debts));
			this._PaymentIdentities = new EntitySet<PaymentIdentity>(new Action<PaymentIdentity>(this.attach_PaymentIdentities), new Action<PaymentIdentity>(this.detach_PaymentIdentities));
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ID", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int ID
		{
			get
			{
				return this._ID;
			}
			set
			{
				if ((this._ID != value))
				{
					this.OnIDChanging(value);
					this.SendPropertyChanging();
					this._ID = value;
					this.SendPropertyChanged("ID");
					this.OnIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_FirstName", DbType="NVarChar(50) NOT NULL", CanBeNull=false)]
		public string FirstName
		{
			get
			{
				return this._FirstName;
			}
			set
			{
				if ((this._FirstName != value))
				{
					this.OnFirstNameChanging(value);
					this.SendPropertyChanging();
					this._FirstName = value;
					this.SendPropertyChanged("FirstName");
					this.OnFirstNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_LastName", DbType="NVarChar(50) NOT NULL", CanBeNull=false)]
		public string LastName
		{
			get
			{
				return this._LastName;
			}
			set
			{
				if ((this._LastName != value))
				{
					this.OnLastNameChanging(value);
					this.SendPropertyChanging();
					this._LastName = value;
					this.SendPropertyChanged("LastName");
					this.OnLastNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_IsDeleted", DbType="bit not null")]
		public bool IsDeleted
		{
			get
			{
				return this._IsDeleted;
			}
			set
			{
				if ((this._IsDeleted != value))
				{
					this.OnIsDeletedChanging(value);
					this.SendPropertyChanging();
					this._IsDeleted = value;
					this.SendPropertyChanged("IsDeleted");
					this.OnIsDeletedChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Email", DbType="NVarChar(50)")]
		public string Email
		{
			get
			{
				return this._Email;
			}
			set
			{
				if ((this._Email != value))
				{
					this.OnEmailChanging(value);
					this.SendPropertyChanging();
					this._Email = value;
					this.SendPropertyChanged("Email");
					this.OnEmailChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="Person_Transaction", Storage="_Credits", ThisKey="ID", OtherKey="CreditorID")]
		public EntitySet<Transaction> Credits
		{
			get
			{
				return this._Credits;
			}
			set
			{
				this._Credits.Assign(value);
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="Person_Transaction1", Storage="_Debts", ThisKey="ID", OtherKey="DebtorID")]
		public EntitySet<Transaction> Debts
		{
			get
			{
				return this._Debts;
			}
			set
			{
				this._Debts.Assign(value);
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="Person_PaymentIdentity", Storage="_PaymentIdentities", ThisKey="ID", OtherKey="PersonID")]
		public EntitySet<PaymentIdentity> PaymentIdentities
		{
			get
			{
				return this._PaymentIdentities;
			}
			set
			{
				this._PaymentIdentities.Assign(value);
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		private void attach_Credits(Transaction entity)
		{
			this.SendPropertyChanging();
			entity.Creditor = this;
		}
		
		private void detach_Credits(Transaction entity)
		{
			this.SendPropertyChanging();
			entity.Creditor = null;
		}
		
		private void attach_Debts(Transaction entity)
		{
			this.SendPropertyChanging();
			entity.Debtor = this;
		}
		
		private void detach_Debts(Transaction entity)
		{
			this.SendPropertyChanging();
			entity.Debtor = null;
		}
		
		private void attach_PaymentIdentities(PaymentIdentity entity)
		{
			this.SendPropertyChanging();
			entity.Person = this;
		}
		
		private void detach_PaymentIdentities(PaymentIdentity entity)
		{
			this.SendPropertyChanging();
			entity.Person = null;
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.PaymentIdentity")]
	public partial class PaymentIdentity : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _Id;
		
		private int _PersonID;
		
		private int _PaymentMethID;
		
		private string _UserName;
		
		private EntityRef<Person> _Person;
		
		private EntityRef<PaymentMethod> _PaymentMethod;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIdChanging(int value);
    partial void OnIdChanged();
    partial void OnPersonIDChanging(int value);
    partial void OnPersonIDChanged();
    partial void OnPaymentMethIDChanging(int value);
    partial void OnPaymentMethIDChanged();
    partial void OnUserNameChanging(string value);
    partial void OnUserNameChanged();
    #endregion
		
		public PaymentIdentity()
		{
			this._Person = default(EntityRef<Person>);
			this._PaymentMethod = default(EntityRef<PaymentMethod>);
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Id", DbType="Int NOT NULL", IsPrimaryKey=true)]
		public int Id
		{
			get
			{
				return this._Id;
			}
			set
			{
				if ((this._Id != value))
				{
					this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_PersonID", DbType="Int NOT NULL")]
		public int PersonID
		{
			get
			{
				return this._PersonID;
			}
			set
			{
				if ((this._PersonID != value))
				{
					if (this._Person.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnPersonIDChanging(value);
					this.SendPropertyChanging();
					this._PersonID = value;
					this.SendPropertyChanged("PersonID");
					this.OnPersonIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_PaymentMethID", DbType="Int NOT NULL")]
		public int PaymentMethID
		{
			get
			{
				return this._PaymentMethID;
			}
			set
			{
				if ((this._PaymentMethID != value))
				{
					if (this._PaymentMethod.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnPaymentMethIDChanging(value);
					this.SendPropertyChanging();
					this._PaymentMethID = value;
					this.SendPropertyChanged("PaymentMethID");
					this.OnPaymentMethIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_UserName", DbType="NVarChar(50) NOT NULL", CanBeNull=false)]
		public string UserName
		{
			get
			{
				return this._UserName;
			}
			set
			{
				if ((this._UserName != value))
				{
					this.OnUserNameChanging(value);
					this.SendPropertyChanging();
					this._UserName = value;
					this.SendPropertyChanged("UserName");
					this.OnUserNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="Person_PaymentIdentity", Storage="_Person", ThisKey="PersonID", OtherKey="ID", IsForeignKey=true)]
		public Person Person
		{
			get
			{
				return this._Person.Entity;
			}
			set
			{
				Person previousValue = this._Person.Entity;
				if (((previousValue != value) 
							|| (this._Person.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._Person.Entity = null;
						previousValue.PaymentIdentities.Remove(this);
					}
					this._Person.Entity = value;
					if ((value != null))
					{
						value.PaymentIdentities.Add(this);
						this._PersonID = value.ID;
					}
					else
					{
						this._PersonID = default(int);
					}
					this.SendPropertyChanged("Person");
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="PaymentMethod_PaymentIdentity", Storage="_PaymentMethod", ThisKey="PaymentMethID", OtherKey="Id", IsForeignKey=true)]
		public PaymentMethod PaymentMethod
		{
			get
			{
				return this._PaymentMethod.Entity;
			}
			set
			{
				PaymentMethod previousValue = this._PaymentMethod.Entity;
				if (((previousValue != value) 
							|| (this._PaymentMethod.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._PaymentMethod.Entity = null;
						previousValue.PaymentIdentities.Remove(this);
					}
					this._PaymentMethod.Entity = value;
					if ((value != null))
					{
						value.PaymentIdentities.Add(this);
						this._PaymentMethID = value.Id;
					}
					else
					{
						this._PaymentMethID = default(int);
					}
					this.SendPropertyChanged("PaymentMethod");
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.PaymentMethod")]
	public partial class PaymentMethod : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _Id;
		
		private string _Name;
		
		private string _PayLinkFormat;
		
		private string _RequestMoneyLinkFormat;
		
		private EntitySet<PaymentIdentity> _PaymentIdentities;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIdChanging(int value);
    partial void OnIdChanged();
    partial void OnNameChanging(string value);
    partial void OnNameChanged();
    partial void OnPayLinkFormatChanging(string value);
    partial void OnPayLinkFormatChanged();
    partial void OnRequestMoneyLinkFormatChanging(string value);
    partial void OnRequestMoneyLinkFormatChanged();
    #endregion
		
		public PaymentMethod()
		{
			this._PaymentIdentities = new EntitySet<PaymentIdentity>(new Action<PaymentIdentity>(this.attach_PaymentIdentities), new Action<PaymentIdentity>(this.detach_PaymentIdentities));
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Id", DbType="Int NOT NULL", IsPrimaryKey=true)]
		public int Id
		{
			get
			{
				return this._Id;
			}
			set
			{
				if ((this._Id != value))
				{
					this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Name", DbType="NChar(50) NOT NULL", CanBeNull=false)]
		public string Name
		{
			get
			{
				return this._Name;
			}
			set
			{
				if ((this._Name != value))
				{
					this.OnNameChanging(value);
					this.SendPropertyChanging();
					this._Name = value;
					this.SendPropertyChanged("Name");
					this.OnNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_PayLinkFormat", DbType="NVarChar(MAX)")]
		public string PayLinkFormat
		{
			get
			{
				return this._PayLinkFormat;
			}
			set
			{
				if ((this._PayLinkFormat != value))
				{
					this.OnPayLinkFormatChanging(value);
					this.SendPropertyChanging();
					this._PayLinkFormat = value;
					this.SendPropertyChanged("PayLinkFormat");
					this.OnPayLinkFormatChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_RequestMoneyLinkFormat", DbType="NVarChar(MAX)")]
		public string RequestMoneyLinkFormat
		{
			get
			{
				return this._RequestMoneyLinkFormat;
			}
			set
			{
				if ((this._RequestMoneyLinkFormat != value))
				{
					this.OnRequestMoneyLinkFormatChanging(value);
					this.SendPropertyChanging();
					this._RequestMoneyLinkFormat = value;
					this.SendPropertyChanged("RequestMoneyLinkFormat");
					this.OnRequestMoneyLinkFormatChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="PaymentMethod_PaymentIdentity", Storage="_PaymentIdentities", ThisKey="Id", OtherKey="PaymentMethID")]
		public EntitySet<PaymentIdentity> PaymentIdentities
		{
			get
			{
				return this._PaymentIdentities;
			}
			set
			{
				this._PaymentIdentities.Assign(value);
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		private void attach_PaymentIdentities(PaymentIdentity entity)
		{
			this.SendPropertyChanging();
			entity.PaymentMethod = this;
		}
		
		private void detach_PaymentIdentities(PaymentIdentity entity)
		{
			this.SendPropertyChanging();
			entity.PaymentMethod = null;
		}
	}
}
#pragma warning restore 1591
