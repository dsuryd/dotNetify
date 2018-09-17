class JobQueueVM {

   _jobID: string = "";
   _jobOutput: string = "";

   $ready() {
      var vm: any = this;
      vm.$on(vm.JobComplete, (iArgs: any) => this.onJobComplete(iArgs));
   }

   add() {
      var vm: any = this;
      vm.NewJob({ ID: vm._jobID(), Start: new Date().toLocaleTimeString() });
      vm._jobID(null);
   }

   onJobComplete(iEventArgs: any) {
      var vm: any = this;
      vm._jobOutput(vm._jobOutput() + "<br/>" + iEventArgs);
   }
}
