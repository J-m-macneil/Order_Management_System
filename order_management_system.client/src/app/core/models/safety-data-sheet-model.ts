export interface SafetyDataSheet {
  safetyDataSheetId: number;
  productId: number;
  fileName: string;
  filePath: string;
  version: string;
  effectiveDate: string;
  uploadedAt: string;
  uploadedByUserId: number;
  uploadedByUserName?: string | null;
  isActive: boolean;
}
