import { HttpClient, provideHttpClient, withFetch } from '@angular/common/http';
import { ApplicationConfig, inject } from '@angular/core';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { provideRouter, withEnabledBlockingInitialNavigation, withInMemoryScrolling } from '@angular/router';
import Aura from '@primeuix/themes/aura';
import { providePrimeNG } from 'primeng/config';
import { appRoutes } from './app.routes';
import { provideTranslateService, TranslateLoader } from '@ngx-translate/core';
import { MessageService } from 'primeng/api';

export const appConfig: ApplicationConfig = {
    providers: [
        provideRouter(appRoutes, withInMemoryScrolling({ anchorScrolling: 'enabled', scrollPositionRestoration: 'enabled' }), withEnabledBlockingInitialNavigation()),
        MessageService,
        provideHttpClient(withFetch()),
        provideAnimationsAsync(),
        providePrimeNG({ theme: { preset: Aura, options: { darkModeSelector: '.app-dark' } } }),
        provideTranslateService({
            loader: {
                provide: TranslateLoader,
                useFactory: () => {
                    const http = inject(HttpClient);
                    return {
                        getTranslation: (lang: string) =>
                            http.get(`/assets/i18n/${lang}.json`)
                    };
                }
            }
        })
    ]
};
