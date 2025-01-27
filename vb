package com.example.dbcruise.service;

import org.springframework.stereotype.Service;
import org.springframework.web.client.RestTemplate;
import org.springframework.beans.factory.annotation.Value;

@Service
public class DbCruiseService {
    private final RestTemplate restTemplate;

    @Value("${dbcruise.service.url}")
    private String serviceUrl;

    public DbCruiseService(RestTemplate restTemplate) {
        this.restTemplate = restTemplate;
    }

    public String getSnapshot() {
        return restTemplate.getForObject(serviceUrl, String.class);
    }
}
