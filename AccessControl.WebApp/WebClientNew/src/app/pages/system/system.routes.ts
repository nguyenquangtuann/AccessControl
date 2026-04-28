import { Routes } from "@angular/router";
import { Appuser } from "./appuser/appuser";
import { Approle } from "./approle/approle";
import { Appgroup } from "./appgroup/appgroup";

export default [
    { path: 'appuser', component: Appuser },
    { path: 'approle', component: Approle },
    { path: 'appgroup', component: Appgroup }
] as Routes;