using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace EFDataAccess.DTO
{
    public class GlobalReportDTO
    {
        public string personType { get; set; }
        //public string FullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string HouseholdName { get; set; }
        public string ChiefPartner { get; set; }
        public string Partner { get; set; }
        public int Age { get; set; }
        public string DateOFBirth { get; set; }
        public string DateOfBirthUnknown { get; set; }
        public string Gender { get; set; }
        public string District { get; set; }
        public string AdministrativePost { get; set; }
        public string NeighborhoodName { get; set; }

        public string Block { get; set; }
        public string ClosePlaceToHome { get; set; }

        public string PrincipalChiefName { get; set; }

        public string FamilyHeadDescription { get; set; } //new

        public string RegistrationDate { get; set; }
        public string InstitutionalAid { get; set; }
        public string InstitutionalAidDetail { get; set; }
        public string communityAid { get; set; }
        public string communityAidDetail { get; set; }
        public string individualAid { get; set; }
        public string FamilyPhoneNumber { get; set; }
        public string AnyoneBedridden { get; set; }
        public string FamilyOriginReference { get; set; }
        public string OtherFamilyOriginRef { get; set; }
        public string ovcDescription { get; set; }
        public string degreeOfKingshipDescription { get; set; }
        public string IsPartSavingGroup { get; set; }
        public string HIVStatus { get; set; }
        public string HIVStatusDetails { get; set; }
        public string NID { get; set; }

        public int P1 { get; set; }
        public int P2 { get; set; }
        public int P3 { get; set; }
        public int P4 { get; set; }
        public int P5 { get; set; }
        public int P6 { get; set; }
        public int P7 { get; set; }
        public int P8 { get; set; }
        public int P9 { get; set; }
        public int P10 { get; set; }
        public int P11 { get; set; }
        public int P12 { get; set; }
        public int P13 { get; set; }
        public int P14 { get; set; }
        public int P15 { get; set; }
        public int P16 { get; set; }
        public int P17 { get; set; }
        public int P18 { get; set; }
        public int P19 { get; set; }
        public int P20 { get; set; }
        public int P21 { get; set; }
        public int P22 { get; set; }
        public int P23 { get; set; }
        public int P24 { get; set; }
        public int P25 { get; set; }
        public int P26 { get; set; }
        public int P27 { get; set; }
        public int P28 { get; set; }
        public int P29 { get; set; }
        public int P30 { get; set; }
        public int P31 { get; set; }
        public int P32 { get; set; }
        public int P33 { get; set; }
        public string A1 { get; set; }
        public string A2 { get; set; }
        public string A3 { get; set; }
        public string A4 { get; set; }
        public string A5 { get; set; }
        public string A6 { get; set; }
        public string A7 { get; set; }
        public string A8 { get; set; }
        public string A9 { get; set; }
        public string A10 { get; set; }
        public string A11 { get; set; }
        public string A12 { get; set; }
        public string A13 { get; set; }
        public string A14 { get; set; }
        public string A15 { get; set; }
        public string A16 { get; set; }
        public string A17 { get; set; }
        public string A18 { get; set; }
        public string A19 { get; set; }
        public string A20 { get; set; }
        public string A21 { get; set; }
        public string A22 { get; set; }
        public string A23 { get; set; }
        public string A24 { get; set; }
        public string A25 { get; set; }
        public string A26 { get; set; }
        public string A27 { get; set; }
        public string A28 { get; set; }
        public string A29 { get; set; }
        public string A30 { get; set; }
        public string A31 { get; set; }
        public string A32 { get; set; }
        public string A33 { get; set; }

        public string ChildStatusDescription { get; set; }
        public string FirstTimeSavingGroup { get; set; }
        public string FE { get; set; }
        public string AN { get; set; }
        public string HAB { get; set; }
        public string ED { get; set; }
        public string SD { get; set; }
        public string APS { get; set; }
        public string PL { get; set; }
        public string DPI { get; set; }
        public string MUACGREEN { get; set; }
        public string MUACYELLOW { get; set; }
        public string MUACRED { get; set; }

        public string ATS { get; set; }
        public string TARV { get; set; }
        public string CCR { get; set; }
        public string SSR { get; set; }
        public string VGB { get; set; }
        public string Others { get; set; }
        public string ReferenceDate { get; set; }
        public string RC_ATS { get; set; }
        public string RC_TARV { get; set; }
        public string RC_CCR { get; set; }
        public string RC_SSR { get; set; }
        public string RC_VGB { get; set; }
        public string RC_Others { get; set; }
        public string HealthAttendedDate { get; set; }
        public string SocialAttendedDate { get; set; }

    public List<Object> values { get; set; } = new List<Object>();

        public List<Object> populatedValues()
        {
            values.Add(personType);
            values.Add(StringUtils.MaskIfConfIsEnabled(FirstName));
            values.Add(StringUtils.MaskIfConfIsEnabled(LastName));
            values.Add(HouseholdName);
            values.Add(ChiefPartner);
            values.Add(Partner);
            values.Add(Age);
            
            values.Add(DateOFBirth);
            values.Add(DateOfBirthUnknown);
           
            values.Add(Gender);
            values.Add(District);
            values.Add(AdministrativePost);
            values.Add(NeighborhoodName);

            values.Add(Block);
            values.Add(ClosePlaceToHome);

            values.Add(PrincipalChiefName);


            values.Add(FamilyHeadDescription);
            values.Add(RegistrationDate);
            values.Add(InstitutionalAid);

            values.Add(InstitutionalAidDetail);
            values.Add(communityAid);
            values.Add(communityAidDetail);
            values.Add(individualAid);
            values.Add(FamilyPhoneNumber);
            values.Add(AnyoneBedridden);
            values.Add(FamilyOriginReference);
            values.Add(OtherFamilyOriginRef);
            values.Add(ovcDescription);
            values.Add(degreeOfKingshipDescription);
            values.Add(IsPartSavingGroup);
            values.Add(HIVStatus);
            values.Add(HIVStatusDetails);
            values.Add(NID);




        

           values.Add(P1);
           values.Add(P2);
           values.Add(P3);
           values.Add(P4);
           values.Add(P5);
           values.Add(P6);
           values.Add(P7);
           values.Add(P8);
           values.Add(P9);
           values.Add(P10);
           values.Add(P11);
           values.Add(P12);
           values.Add(P13);
           values.Add(P14);
           values.Add(P15);
           values.Add(P16);
           values.Add(P17);
           values.Add(P18);
           values.Add(P19);
           values.Add(P20);
           values.Add(P21);
           values.Add(P22);
           values.Add(P23);
           values.Add(P24);
           values.Add(P25);
           values.Add(P26);
           values.Add(P27);
           values.Add(P28);
           values.Add(P29);
           values.Add(P30);
           values.Add(P31);
           values.Add(P32);
           values.Add(P33);
           values.Add(A1);
           values.Add(A2);
           values.Add(A3);
           values.Add(A4);
           values.Add(A5);
           values.Add(A6);
           values.Add(A7);
           values.Add(A8);
           values.Add(A9);
           values.Add(A10);
           values.Add(A11);
           values.Add(A12);
           values.Add(A13);
           values.Add(A14);
           values.Add(A15);
           values.Add(A16);
           values.Add(A17);
           values.Add(A18);
           values.Add(A19);
           values.Add(A20);
           values.Add(A21);
           values.Add(A22);
           values.Add(A23);
           values.Add(A24);
           values.Add(A25);
           values.Add(A26);
           values.Add(A27);
           values.Add(A28);
           values.Add(A29);
           values.Add(A30);
           values.Add(A31);
           values.Add(A32);
           values.Add(A33);

           values.Add(ChildStatusDescription);
            values.Add(FirstTimeSavingGroup);
            values.Add(FE);
            values.Add(AN);
            values.Add(HAB);
            values.Add(ED);
            values.Add(SD);
            values.Add(APS);
            values.Add(PL);
            values.Add(DPI);
            values.Add(MUACGREEN);
            values.Add(MUACYELLOW);
            values.Add(MUACRED);

            values.Add(ATS);
           values.Add(TARV);
           values.Add(CCR);
           values.Add(SSR);
           values.Add(VGB);
           values.Add(Others);
            values.Add(ReferenceDate);
            values.Add(RC_ATS);
           values.Add(RC_TARV);
           values.Add(RC_CCR);
           values.Add(RC_SSR);
           values.Add(RC_VGB);
           values.Add(RC_Others);


            values.Add(HealthAttendedDate);
            values.Add(SocialAttendedDate);

            return values;
        }

    }
}
