package com.example.manualapi.controller;

import com.example.manualapi.model.RestrictedSecurityRequest;
import com.example.manualapi.model.RestrictedSecurityResponse;
import com.example.manualapi.service.RestrictedSecurityService;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping("/api/manual")
public class ManualInputController {

    @Autowired
    private RestrictedSecurityService restrictedSecurityService;

    @PostMapping("/restricted-securities")
    public List<RestrictedSecurityResponse> getRestrictedSecurities(@RequestBody RestrictedSecurityRequest request) {
        return restrictedSecurityService.getRestrictedSecurities(request);
    }
}


package com.example.manualapi.model;

import lombok.Data;

import java.util.List;

@Data
public class RestrictedSecurityRequest {
    private List<String> restrictionTypeCodes;
    private String dealStatus;
    private String region;
}

package com.example.manualapi.model;

import lombok.AllArgsConstructor;
import lombok.Data;

@Data
@AllArgsConstructor
public class RestrictedSecurityResponse {
    private String instrumentName;
    private String instrumentIsin;
    private String restrictionTypeName;
}


package com.example.manualapi.service;

import com.example.manualapi.model.RestrictedSecurityRequest;
import com.example.manualapi.model.RestrictedSecurityResponse;
import org.springframework.stereotype.Service;

import java.util.ArrayList;
import java.util.List;

@Service
public class RestrictedSecurityService {

    public List<RestrictedSecurityResponse> getRestrictedSecurities(RestrictedSecurityRequest request) {
        // Mock Data: Hardcoded list of securities
        List<RestrictedSecurityResponse> allSecurities = List.of(
                new RestrictedSecurityResponse("Apple Inc", "US0378331005", "Voting Rights"),
                new RestrictedSecurityResponse("Tesla Inc", "US88160R1014", "Tender Offer"),
                new RestrictedSecurityResponse("Google LLC", "US02079K1079", "Ownership Restrictions")
        );

        // Filter based on the request parameters
        List<RestrictedSecurityResponse> filteredSecurities = new ArrayList<>();
        for (RestrictedSecurityResponse security : allSecurities) {
            if (request.getRestrictionTypeCodes().contains("21") 
                    && request.getDealStatus().equalsIgnoreCase("ACTIVE")) {
                filteredSecurities.add(security);
            }
        }

        return filteredSecurities;
    }
}


server.port=8080
spring.application.name=ManualAPIExample


