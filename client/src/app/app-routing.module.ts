import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from './guard/auth.guard';
import { PreventUnsavedChangesGuard } from './guard/prevent-unsaved-changes.guard';
import { HomeComponent } from './home/home.component';
import { ListsComponent } from './lists/lists.component';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MessagesComponent } from './messages/messages.component';
import { MemberDetailedResolver } from './resolvers/member-detailed.resolver';

const routes: Routes = [
  {path:'', component : HomeComponent},
  {
    path:'',
    runGuardsAndResolvers:'always',
    canActivate :[AuthGuard],
    children:[
      {path:'members', component : MemberListComponent},
      {path:'members/:username', component : MemberDetailComponent,resolve :{member:MemberDetailedResolver}},
      {path:'member/edit', component : MemberEditComponent, canDeactivate :[PreventUnsavedChangesGuard]},
      {path:'lists', component : ListsComponent},
      {path:'messages', component : MessagesComponent},
    ]
  },
 
  {path:'**', component : HomeComponent, pathMatch:'full'},
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
