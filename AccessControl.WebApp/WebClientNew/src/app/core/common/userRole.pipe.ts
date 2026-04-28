import { NgModule, Pipe, PipeTransform } from "@angular/core";
import { SystemConstants } from "./system.constants";

@Pipe({
  name: "userRoles",
  standalone: true,
  pure: false //
})
export class UserRoles implements PipeTransform {
  transform(value?: any, ...args: any[]) {
    let userRoles: string[] = JSON.parse(localStorage.getItem(SystemConstants.CURRENT_USER_ROLE)!);
    return userRoles?.includes(value) ?? false;
  }
}
