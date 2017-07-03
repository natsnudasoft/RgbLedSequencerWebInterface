(function () {
    "use strict";
    angular
        .module('app.rgbLedSequencer')
        .service('rgbLedSequencerDataService', ['$http', '$log', dataService]);

    function dataService($http, $log) {
        this.enqueueSequence = enqueueSequence;
        this.getSequenceQueuePage = getSequenceQueuePage;
        this.sendNextSequence = sendNextSequence;

        function enqueueSequence(sequence) {
            return $http.post('/RgbLedSequencer/EnqueueSequence', JSON.stringify(sequence))
                .catch(enqueueSequenceFailed);

            function enqueueSequenceFailed(error) {
                $log.error('XHR Failed for enqueueSequence.');
                throw error;
            }
        }

        function getSequenceQueuePage(pageNumber) {
            return $http.get('/RgbLedSequencer/SequenceQueue?pageNumber=' + pageNumber)
                .then(getSequenceQueuePageSuccess)
                .catch(getSequenceQueuePageFailed);

            function getSequenceQueuePageSuccess(status) {
                return status.data;
            }

            function getSequenceQueuePageFailed(error) {
                $log.error('XHR Failed for getSequenceQueuePage.');
                throw error;
            }
        }

        function sendNextSequence() {
            return $http.post('/RgbLedSequencer/SendNextSequence')
                .catch(sendNextSequenceFailed);
            
            function sendNextSequenceFailed(error) {
                $log.error('XHR Failed for sendNextSequence.');
                throw error;
            }
        }
    }
})();
