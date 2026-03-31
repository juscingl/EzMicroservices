{{- define "webbff.name" -}}
{{- default .Chart.Name .Values.nameOverride | trunc 63 | trimSuffix "-" -}}
{{- end -}}

{{- define "webbff.fullname" -}}
{{- $name := default .Chart.Name .Values.nameOverride -}}
{{- printf "%s-%s" .Release.Name $name | trunc 63 | trimSuffix "-" -}}
{{- end -}}

{{- define "webbff.labels" -}}
app.kubernetes.io/name: {{ include "webbff.name" . }}
helm.sh/chart: {{ .Chart.Name }}-{{ .Chart.Version | replace "+" "_" }}
app.kubernetes.io/instance: {{ .Release.Name }}
app.kubernetes.io/managed-by: Helm
{{- end -}}

{{- define "webbff.selectorLabels" -}}
app.kubernetes.io/name: {{ include "webbff.name" . }}
app.kubernetes.io/instance: {{ .Release.Name }}
{{- end -}}
