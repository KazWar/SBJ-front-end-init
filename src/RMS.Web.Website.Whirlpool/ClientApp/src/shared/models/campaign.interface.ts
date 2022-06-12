export default interface Campaign {
  campaignId: number;
  campaignName: string;
  campaignDescription: string;
  campaignCode: number;
  localeDescription: string;
  version: string;
  startDate: Date;
  endDate: Date;
  thumbnail?: string; // base64-encoded string
}
