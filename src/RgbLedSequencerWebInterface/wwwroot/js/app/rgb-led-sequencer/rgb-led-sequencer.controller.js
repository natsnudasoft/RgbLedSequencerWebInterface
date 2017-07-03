(function () {
    "use strict";
    angular
        .module('app.rgbLedSequencer')
        .controller('rgbLedSequencerController', ['$interval', 'rgbLedSequencerDataService', RgbLedSequencerController ]);

    function RgbLedSequencerController($interval, rgbLedSequencerDataService) {
        var vm = this;
        vm.sequenceCount = 10;
        vm.rgbLedCount = 5;
        vm.maxStepCount = 770;
        vm.maxStepDelay = 65535;

        vm.totalItemCount = 0;
        vm.currentPage = 1;
        vm.pageCount = 1;
        vm.pageSequenceQueue = [];
        vm.pageLoading = false;

        vm.sequence = [];
        vm.sequenceName = '';
        vm.sequenceIndex = 0;
        vm.stepCount = 100;
        vm.currentStep = 1;
        vm.currentPreviewStep = 1;
        vm.addingSequence = false;
        vm.hasError = false;
        vm.enqueueSuccess = false;
        vm.errorMessage = '';
        vm.syncSteps = syncSteps;
        vm.enqueueSequence = enqueueSequence;
        vm.sequenceQueuePageChanged = sequenceQueuePageChanged;
        vm.getLocalDate = getLocalDate;
        vm.sendNextSequence = sendNextSequence;

        setInitialSequence();
        syncSteps();
        createPreview();
        sequenceQueuePageChanged();

        function syncSteps() {
            var i;
            var j;
            vm.stepCount = Math.max(1, Math.min(vm.stepCount, vm.maxStepCount));
            if (vm.currentPreviewStep > vm.stepCount) {
                vm.currentPreviewStep = 1;
            }
            if (vm.currentStep > vm.stepCount) {
                vm.currentStep = vm.stepCount;
            }

            var difference = vm.stepCount - vm.sequence.length;
            if (difference > 0) {
                for (i = 0; i < difference; ++i) {
                    var colors = [];
                    for (j = 0; j < vm.rgbLedCount; ++j) {
                        colors.push({ index: j, value: '#000000' });
                    }
                    var newStep = { colors: colors, stepDelay: 0 };
                    vm.sequence.push(newStep);
                }
            } else if (difference < 0) {
                for (i = 0; i > difference; --i) {
                    vm.sequence.pop();
                }
            }
        }

        function enqueueSequence() {
            vm.hasError = false;
            vm.enqueueSuccess = false;
            vm.addingSequence = true;
            var sequence = {
                sequenceName: vm.sequenceName,
                sequenceIndex: vm.sequenceIndex,
                steps: vm.sequence
            };
            rgbLedSequencerDataService.enqueueSequence(sequence)
                .then(enqueueSequenceComplete)
                .catch(enqueueSequenceError);

            function enqueueSequenceComplete() {
                vm.enqueueSuccess = true;
                vm.addingSequence = false;
                sequenceQueuePageChanged();
            }

            function enqueueSequenceError(error) {
                vm.hasError = true;
                vm.addingSequence = false;
                vm.errorMessage = error.statusText;
            }
        }

        function sendNextSequence() {
            rgbLedSequencerDataService.sendNextSequence();
        }

        function sequenceQueuePageChanged() {
            vm.pageLoading = true;
            rgbLedSequencerDataService.getSequenceQueuePage(vm.currentPage)
                .then(getSequenceQueuePageComplete);

            function getSequenceQueuePageComplete(sequenceQueueViewModel) {
                if (sequenceQueueViewModel.pageNumber === vm.currentPage) {
                    vm.totalItemCount = sequenceQueueViewModel.totalItemCount;
                    vm.pageNumber = sequenceQueueViewModel.pageNumber;
                    vm.pageSequenceQueue = sequenceQueueViewModel.pageSequenceQueue;
                    vm.pageLoading = false;
                }
            }
        }

        function createPreview() {
            var currentStepTime = 0;
            var previousTime = new Date().getTime();
            var previewInterval;
            if (!angular.isDefined(previewInterval)) {
                previewInterval = $interval(previewUpdate, 100);
            }

            vm.$onDestroy = function () {
                if (angular.isDefined(previewInterval)) {
                    $interval.cancel(previewInterval);
                }
            };

            function previewUpdate() {
                if (vm.stepCount >= 1 && vm.stepCount <= vm.maxStepCount) {
                    var currentTime = new Date().getTime();
                    currentStepTime += currentTime - previousTime;
                    previousTime = currentTime;
                    var currentStepDelay = vm.sequence[vm.currentPreviewStep - 1].stepDelay;
                    if (currentStepTime > currentStepDelay) {
                        currentStepTime = 0;
                        ++vm.currentPreviewStep;
                        if (vm.currentPreviewStep > vm.stepCount) {
                            vm.currentPreviewStep = 1;
                        }
                    }
                }
            }
        }

        function setInitialSequence() {
            vm.sequence = [
                { colors: [{ index: 0, value: '#FF0000' }, { index: 1, value: '#000000' }, { index: 2, value: '#000000' }, { index: 3, value: '#000000' }, { index: 4, value: '#000000' }], stepDelay: 0 },
                { colors: [{ index: 0, value: '#000000' }, { index: 1, value: '#FF0000' }, { index: 2, value: '#000000' }, { index: 3, value: '#000000' }, { index: 4, value: '#000000' }], stepDelay: 0 },
                { colors: [{ index: 0, value: '#000000' }, { index: 1, value: '#000000' }, { index: 2, value: '#FF0000' }, { index: 3, value: '#000000' }, { index: 4, value: '#000000' }], stepDelay: 0 },
                { colors: [{ index: 0, value: '#000000' }, { index: 1, value: '#000000' }, { index: 2, value: '#000000' }, { index: 3, value: '#FF0000' }, { index: 4, value: '#000000' }], stepDelay: 0 },
                { colors: [{ index: 0, value: '#000000' }, { index: 1, value: '#000000' }, { index: 2, value: '#000000' }, { index: 3, value: '#000000' }, { index: 4, value: '#FF0000' }], stepDelay: 0 },
                { colors: [{ index: 0, value: '#000000' }, { index: 1, value: '#000000' }, { index: 2, value: '#000000' }, { index: 3, value: '#FF0000' }, { index: 4, value: '#000000' }], stepDelay: 0 },
                { colors: [{ index: 0, value: '#000000' }, { index: 1, value: '#000000' }, { index: 2, value: '#FF0000' }, { index: 3, value: '#000000' }, { index: 4, value: '#000000' }], stepDelay: 0 },
                { colors: [{ index: 0, value: '#000000' }, { index: 1, value: '#FF0000' }, { index: 2, value: '#000000' }, { index: 3, value: '#000000' }, { index: 4, value: '#000000' }], stepDelay: 0 }
            ];
            vm.stepCount = vm.sequence.length;
        }

        function getLocalDate(unixMilliseconds) {
            return new Date(unixMilliseconds).toLocaleString();
        }
    }
})();
