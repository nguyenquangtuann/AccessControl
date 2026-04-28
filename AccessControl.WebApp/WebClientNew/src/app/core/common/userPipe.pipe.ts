import { Pipe, PipeTransform } from '@angular/core';
import { SystemConstants } from 'src/app/core/common/system.constants';

const users = JSON.parse(localStorage.getItem(SystemConstants.USERS_PIPE)!);

@Pipe({
  name: 'userPipe',
  standalone: true
})
export class UserPipe implements PipeTransform {
  transform(value: any): string {
    let newStr = users?.find((x: any) => x.id == value);
    return newStr ? newStr.userName : '';
  }
}
