// Unless explicitly stated otherwise all files in this repository are licensed under the Apache 2 License.
// This product includes software developed at Datadog (https://www.datadoghq.com/). Copyright 2022 Datadog, Inc.

#pragma once

#include "GCBaseRawSample.h"
#include "RawSample.h"
#include "Sample.h"

class RawStopTheWorldSample : public GCBaseRawSample
{
public:
    RawStopTheWorldSample() = default;

    RawStopTheWorldSample(RawStopTheWorldSample&& other) = default;
    RawStopTheWorldSample& operator=(RawStopTheWorldSample&& other) noexcept = default;

    // Duration is the suspension time so default sample value
    void DoAdditionalTransform(std::shared_ptr<Sample> sample, uint32_t valueOffset) const override
    {
        // set event type
        sample->AddLabel(Label(Sample::TimelineEventTypeLabel, Sample::TimelineEventTypeStopTheWorld));
    }
};