mergeInto(LibraryManager.library, {
  StartListening: function () {
    console.log("£££ jslib is working");
    Start();
  },

  StopListening: function () {
    Stop();
  },
});