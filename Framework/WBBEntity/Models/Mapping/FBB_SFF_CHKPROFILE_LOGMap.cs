using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBB_SFF_CHKPROFILE_LOGMap : EntityTypeConfiguration<FBB_SFF_CHKPROFILE_LOG>
    {
        public FBB_SFF_CHKPROFILE_LOGMap()
        {
            // Primary Key
            this.HasKey(t => new { t.SFF_CHKPROFILE_ID });

            this.Property(t => t.SFF_CHKPROFILE_ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            // Properties
            this.Property(t => t.INAPPLICATION).HasMaxLength(50);

            this.Property(t => t.INMOBILENO).HasMaxLength(30);
            this.Property(t => t.INIDCARDNO).HasMaxLength(30);
            this.Property(t => t.INIDCARDTYPE).HasMaxLength(255);
            this.Property(t => t.OUTBUILDINGNAME).HasMaxLength(80);
            this.Property(t => t.OUTFLOOR).HasMaxLength(30);
            this.Property(t => t.OUTHOUSENUMBER).HasMaxLength(30);
            this.Property(t => t.OUTMOO).HasMaxLength(30);
            this.Property(t => t.OUTSOI).HasMaxLength(50);
            this.Property(t => t.OUTSTREETNAME).HasMaxLength(50);
            this.Property(t => t.OUTEMAILBILLTO).HasMaxLength(100);
            this.Property(t => t.OUTPROVINCE).HasMaxLength(50);
            this.Property(t => t.OUTAMPHUR).HasMaxLength(50);
            this.Property(t => t.OUTTUMBOL).HasMaxLength(50);
            this.Property(t => t.OUTACCOUNTNUMBER).HasMaxLength(100);
            this.Property(t => t.OUTSERVICEACCOUNTNUMBER).HasMaxLength(30);
            this.Property(t => t.OUTBILLINGACCOUNTNUMBER).HasMaxLength(30);
            this.Property(t => t.OUTBIRTHDATE).HasMaxLength(10);
            this.Property(t => t.OUTACCOUNTNAME).HasMaxLength(100);
            this.Property(t => t.OUTPRIMARYCONTACTFIRSTNAME).HasMaxLength(50);
            this.Property(t => t.OUTCONTACTLASTNAME).HasMaxLength(50);
            this.Property(t => t.ERRORMESSAGE).HasMaxLength(2000);
            this.Property(t => t.OUTPRODUCTNAME).HasMaxLength(255);
            this.Property(t => t.OUTSERVICEYEAR).HasMaxLength(255);
            this.Property(t => t.CREATED_BY).HasMaxLength(30);
            this.Property(t => t.OUTMOOBAN).HasMaxLength(80);
            this.Property(t => t.OUTACCOUNTSUBCATEGORY).HasMaxLength(50);
            this.Property(t => t.OUTPARAMETER2).HasMaxLength(1);
            this.Property(t => t.OUTPOSTALCODE).HasMaxLength(5);
            this.Property(t => t.OUTEMAIL).HasMaxLength(255);
            this.Property(t => t.OUTTITLE).HasMaxLength(50);
            this.Property(t => t.OUTFULLADDRESS).HasMaxLength(1000);
            this.Property(t => t.OUTMOBILESEGMENT).HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("FBB_SFF_CHKPROFILE_LOG", "WBB");
            this.Property(t => t.SFF_CHKPROFILE_ID).HasColumnName("SFF_CHKPROFILE_ID");
            this.Property(t => t.INAPPLICATION).HasColumnName("INAPPLICATION");
            this.Property(t => t.INMOBILENO).HasColumnName("INMOBILENO");
            this.Property(t => t.INIDCARDNO).HasColumnName("INIDCARDNO");
            this.Property(t => t.INIDCARDTYPE).HasColumnName("INIDCARDTYPE");
            this.Property(t => t.OUTBUILDINGNAME).HasColumnName("OUTBUILDINGNAME");
            this.Property(t => t.OUTFLOOR).HasColumnName("OUTFLOOR");
            this.Property(t => t.OUTHOUSENUMBER).HasColumnName("OUTHOUSENUMBER");
            this.Property(t => t.OUTMOO).HasColumnName("OUTMOO");
            this.Property(t => t.OUTSOI).HasColumnName("OUTSOI");
            this.Property(t => t.OUTSTREETNAME).HasColumnName("OUTSTREETNAME");
            this.Property(t => t.OUTEMAILBILLTO).HasColumnName("OUTEMAILBILLTO");
            this.Property(t => t.OUTPROVINCE).HasColumnName("OUTPROVINCE");
            this.Property(t => t.OUTAMPHUR).HasColumnName("OUTAMPHUR");
            this.Property(t => t.OUTTUMBOL).HasColumnName("OUTTUMBOL");
            this.Property(t => t.OUTACCOUNTNUMBER).HasColumnName("OUTACCOUNTNUMBER");
            this.Property(t => t.OUTSERVICEACCOUNTNUMBER).HasColumnName("OUTSERVICEACCOUNTNUMBER");
            this.Property(t => t.OUTBILLINGACCOUNTNUMBER).HasColumnName("OUTBILLINGACCOUNTNUMBER");
            this.Property(t => t.OUTBIRTHDATE).HasColumnName("OUTBIRTHDATE");
            this.Property(t => t.OUTACCOUNTNAME).HasColumnName("OUTACCOUNTNAME");
            this.Property(t => t.OUTPRIMARYCONTACTFIRSTNAME).HasColumnName("OUTPRIMARYCONTACTFIRSTNAME");
            this.Property(t => t.OUTCONTACTLASTNAME).HasColumnName("OUTCONTACTLASTNAME");
            this.Property(t => t.ERRORMESSAGE).HasColumnName("ERRORMESSAGE");
            this.Property(t => t.OUTPRODUCTNAME).HasColumnName("OUTPRODUCTNAME");
            this.Property(t => t.OUTSERVICEYEAR).HasColumnName("OUTSERVICEYEAR");
            this.Property(t => t.CREATED_BY).HasColumnName("CREATED_BY");
            this.Property(t => t.CREATED_DATE).HasColumnName("CREATED_DATE");
            this.Property(t => t.OUTMOOBAN).HasColumnName("OUTMOOBAN");
            this.Property(t => t.OUTACCOUNTSUBCATEGORY).HasColumnName("OUTACCOUNTSUBCATEGORY");
            this.Property(t => t.OUTPARAMETER2).HasColumnName("OUTPARAMETER2");
            this.Property(t => t.OUTPOSTALCODE).HasColumnName("OUTPOSTALCODE");
            this.Property(t => t.OUTEMAIL).HasColumnName("OUTEMAIL");
            this.Property(t => t.OUTTITLE).HasColumnName("OUTTITLE");
            this.Property(t => t.TRANSACTION_ID).HasColumnName("TRANSACTION_ID");
            this.Property(t => t.OUTFULLADDRESS).HasColumnName("OUTFULLADDRESS");
            this.Property(t => t.OUTMOBILESEGMENT).HasColumnName("OUTMOBILESEGMENT");
        }
    }
}
