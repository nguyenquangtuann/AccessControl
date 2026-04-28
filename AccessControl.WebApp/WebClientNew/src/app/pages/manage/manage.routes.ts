import { Routes } from "@angular/router";
import { Department } from "./department/department";
import { Employee } from "./employee/employee";
import { Regency } from "./regency/regency";

export default [
    { path: 'employee', component: Employee },
    { path: 'department', component: Department },
    { path: 'regency', component: Regency }
] as Routes;