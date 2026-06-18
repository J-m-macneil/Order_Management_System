import { environment } from '../../../environments/environment';

type RuntimeConfig = {
  apiBaseUrl?: string;
};

declare global {
  interface Window {
    __backConfig?: RuntimeConfig;
  }
}

export const apiBaseUrl: string =
  window.__backConfig?.apiBaseUrl || environment.apiBaseUrl;
